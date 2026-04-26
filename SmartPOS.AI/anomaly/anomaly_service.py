import pandas as pd
import numpy as np
from sklearn.ensemble import IsolationForest
from sklearn.preprocessing import StandardScaler
import joblib
import os
import sys

sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
from db import query_to_df, get_connection

MODEL_PATH = os.path.join(os.path.dirname(__file__), "..", "models", "anomaly_model.pkl")

def load_transaction_data() -> pd.DataFrame:
    sql = """
        SELECT
            t.TransactionId,
            t.TotalAmount,
            t.Subtotal,
            t.TaxAmount,
            t.DiscountAmount,
            COUNT(ti.TransactionItemId)  AS item_count,
            SUM(ti.Quantity)             AS total_quantity,
            DATEPART(HOUR, t.CreatedAt)  AS hour_of_day,
            DATEPART(WEEKDAY, t.CreatedAt) AS day_of_week
        FROM Transactions t
        JOIN TransactionItems ti ON ti.TransactionId = t.TransactionId
        WHERE t.Status = 'Completed'
        GROUP BY
            t.TransactionId, t.TotalAmount, t.Subtotal,
            t.TaxAmount, t.DiscountAmount,
            DATEPART(HOUR, t.CreatedAt),
            DATEPART(WEEKDAY, t.CreatedAt)
    """
    return query_to_df(sql)

def train_anomaly_model():
    df = load_transaction_data()

    if len(df) < 10:
        return {"error": "Not enough transactions to train. Need at least 10."}

    features = ["TotalAmount", "item_count", "total_quantity", "hour_of_day", "DiscountAmount"]
    X = df[features].values

    scaler   = StandardScaler()
    X_scaled = scaler.fit_transform(X)

    model = IsolationForest(contamination=0.05, random_state=42, n_estimators=100)
    model.fit(X_scaled)

    joblib.dump({"model": model, "scaler": scaler}, MODEL_PATH)

    return {
        "status":      "trained",
        "data_points": len(df)
    }

def detect_anomalies():
    df = load_transaction_data()

    if len(df) < 10:
        return {"error": "Not enough transactions for anomaly detection."}

    if not os.path.exists(MODEL_PATH):
        result = train_anomaly_model()
        if "error" in result:
            return result

    saved    = joblib.load(MODEL_PATH)
    model    = saved["model"]
    scaler   = saved["scaler"]

    features = ["TotalAmount", "item_count", "total_quantity", "hour_of_day", "DiscountAmount"]
    X        = df[features].values
    X_scaled = scaler.transform(X)

    df["anomaly_score"] = model.decision_function(X_scaled)
    df["is_anomaly"]    = model.predict(X_scaled) == -1

    anomalies = df[df["is_anomaly"] == True]

    conn   = get_connection()
    cursor = conn.cursor()

    for _, row in anomalies.iterrows():
        cursor.execute(
            "UPDATE Transactions SET IsAnomaly = 1, AnomalyScore = ? WHERE TransactionId = ?",
            float(row["anomaly_score"]), int(row["TransactionId"])
        )
        cursor.execute(
            """IF NOT EXISTS (SELECT 1 FROM AnomalyLogs WHERE TransactionId = ?)
               INSERT INTO AnomalyLogs (TransactionId, AnomalyScore)
               VALUES (?, ?)""",
            int(row["TransactionId"]),
            int(row["TransactionId"]),
            float(row["anomaly_score"])
        )
    conn.commit()
    conn.close()

    return {
        "status":            "completed",
        "total_analyzed":    len(df),
        "anomalies_found":   len(anomalies),
        "anomaly_rate":      round(len(anomalies) / len(df) * 100, 2)
    }