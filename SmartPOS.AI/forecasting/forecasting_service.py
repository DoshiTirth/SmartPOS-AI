import pandas as pd
import numpy as np
from sklearn.linear_model import LinearRegression
from sklearn.preprocessing import StandardScaler
import joblib
import os
import sys

sys.path.append(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
from db import query_to_df

MODEL_PATH = os.path.join(os.path.dirname(__file__), "..", "models", "forecasting_model.pkl")

def load_sales_data() -> pd.DataFrame:
    sql = """
        SELECT
            CAST(t.CreatedAt AS DATE)   AS sale_date,
            SUM(t.TotalAmount)          AS daily_revenue,
            COUNT(*)                    AS transaction_count
        FROM Transactions t
        WHERE t.Status = 'Completed'
          AND t.CreatedAt >= DATEADD(DAY, -90, GETUTCDATE())
        GROUP BY CAST(t.CreatedAt AS DATE)
        ORDER BY sale_date ASC
    """
    return query_to_df(sql)

def prepare_features(df: pd.DataFrame) -> pd.DataFrame:
    df = df.copy()
    df["sale_date"]  = pd.to_datetime(df["sale_date"])
    df["day_of_week"] = df["sale_date"].dt.dayofweek
    df["day_of_month"] = df["sale_date"].dt.day
    df["month"]       = df["sale_date"].dt.month
    df["day_index"]   = range(len(df))
    return df

def train_model():
    df = load_sales_data()

    if len(df) < 7:
        return {"error": "Not enough data to train. Need at least 7 days of sales."}

    df = prepare_features(df)

    features = ["day_index", "day_of_week", "day_of_month", "month"]
    X = df[features].values
    y = df["daily_revenue"].values

    scaler = StandardScaler()
    X_scaled = scaler.fit_transform(X)

    model = LinearRegression()
    model.fit(X_scaled, y)

    joblib.dump({"model": model, "scaler": scaler}, MODEL_PATH)

    return {
        "status":       "trained",
        "data_points":  len(df),
        "date_range":   f"{df['sale_date'].min().date()} to {df['sale_date'].max().date()}"
    }

def forecast_next_days(days: int = 7) -> list:
    if not os.path.exists(MODEL_PATH):
        result = train_model()
        if "error" in result:
            return []

    saved     = joblib.load(MODEL_PATH)
    model     = saved["model"]
    scaler    = saved["scaler"]

    df = load_sales_data()
    df = prepare_features(df)

    last_index = len(df)
    last_date  = pd.to_datetime(df["sale_date"].max())

    forecasts = []
    for i in range(1, days + 1):
        future_date  = last_date + pd.Timedelta(days=i)
        feature_row  = [[
            last_index + i,
            future_date.dayofweek,
            future_date.day,
            future_date.month
        ]]
        X_scaled     = scaler.transform(feature_row)
        predicted    = model.predict(X_scaled)[0]
        predicted    = max(0, round(float(predicted), 2))

        forecasts.append({
            "date":           str(future_date.date()),
            "predicted_sales": predicted
        })

    return forecasts