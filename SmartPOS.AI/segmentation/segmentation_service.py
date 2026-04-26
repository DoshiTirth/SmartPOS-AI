import pandas as pd
import numpy as np
from sklearn.cluster import KMeans
from sklearn.preprocessing import StandardScaler
import joblib
import os
import sys

sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
from db import query_to_df, get_connection

MODEL_PATH = os.path.join(os.path.dirname(__file__), "..", "models", "segmentation_model.pkl")

def load_customer_data() -> pd.DataFrame:
    sql = """
        SELECT
            c.CustomerId,
            c.FullName,
            COUNT(t.TransactionId)      AS total_transactions,
            ISNULL(SUM(t.TotalAmount), 0)  AS total_spent,
            ISNULL(AVG(t.TotalAmount), 0)  AS avg_transaction,
            ISNULL(MAX(t.TotalAmount), 0)  AS max_transaction,
            DATEDIFF(DAY, MAX(t.CreatedAt), GETUTCDATE()) AS days_since_last_purchase
        FROM Customers c
        LEFT JOIN Transactions t ON t.CustomerId = c.CustomerId
            AND t.Status = 'Completed'
        GROUP BY c.CustomerId, c.FullName
        HAVING COUNT(t.TransactionId) > 0
    """
    return query_to_df(sql)

def get_segment_label(cluster_id: int, cluster_centers: np.ndarray) -> str:
    center = cluster_centers[cluster_id]
    total_spent = center[1]
    frequency   = center[0]
    recency     = center[3]

    if total_spent > np.percentile(cluster_centers[:, 1], 75):
        return "High Value"
    elif recency > 30:
        return "At Risk"
    elif frequency < 2:
        return "Occasional"
    elif total_spent > np.percentile(cluster_centers[:, 1], 50):
        return "Regular"
    else:
        return "New"

def run_segmentation():
    df = load_customer_data()

    if len(df) < 3:
        return {"error": "Not enough customers to segment. Need at least 3."}

    features = ["total_transactions", "total_spent", "avg_transaction", "days_since_last_purchase"]
    X = df[features].values

    scaler   = StandardScaler()
    X_scaled = scaler.fit_transform(X)

    n_clusters = min(4, len(df))
    kmeans     = KMeans(n_clusters=n_clusters, random_state=42, n_init=10)
    df["cluster"] = kmeans.fit_predict(X_scaled)

    joblib.dump({"model": kmeans, "scaler": scaler}, MODEL_PATH)

    df["segment"] = df["cluster"].apply(
        lambda c: get_segment_label(c, kmeans.cluster_centers_)
    )

    conn   = get_connection()
    cursor = conn.cursor()
    for _, row in df.iterrows():
        cursor.execute(
            "UPDATE Customers SET Segment = ? WHERE CustomerId = ?",
            row["segment"], int(row["CustomerId"])
        )
        cursor.execute(
            """INSERT INTO CustomerSegments (CustomerId, Segment)
               VALUES (?, ?)""",
            int(row["CustomerId"]), row["segment"]
        )
    conn.commit()
    conn.close()

    segment_summary = df.groupby("segment").agg(
        customer_count=("CustomerId", "count"),
        avg_spent=("total_spent", "mean")
    ).reset_index()

    return {
        "status":   "completed",
        "segments": segment_summary.to_dict(orient="records"),
        "total_customers_segmented": len(df)
    }