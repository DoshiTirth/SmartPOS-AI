import pyodbc
import pandas as pd
from dotenv import load_dotenv
import os

load_dotenv()

def get_connection():
    conn_str = os.getenv("DB_CONNECTION_STRING")
    return pyodbc.connect(conn_str)

def query_to_df(sql: str) -> pd.DataFrame:
    conn = get_connection()
    df = pd.read_sql(sql, conn)
    conn.close()
    return df