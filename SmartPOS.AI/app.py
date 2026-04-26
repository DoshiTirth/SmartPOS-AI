from flask import Flask, jsonify, request
from flask_cors import CORS
from dotenv import load_dotenv
import os

load_dotenv()

from forecasting.forecasting_service import forecast_next_days, train_model
from segmentation.segmentation_service import run_segmentation
from anomaly.anomaly_service import detect_anomalies, train_anomaly_model

app = Flask(__name__)
CORS(app)

#  Health Check 
@app.route("/health", methods=["GET"])
def health():
    return jsonify({"status": "ok", "service": "SmartPOS AI Microservice"})

#  Forecasting 
@app.route("/forecast", methods=["GET"])
def forecast():
    days = request.args.get("days", 7, type=int)
    result = forecast_next_days(days)
    return jsonify(result)

@app.route("/forecast/train", methods=["POST"])
def train_forecast():
    result = train_model()
    return jsonify(result)

#  Segmentation 
@app.route("/segmentation/run", methods=["POST"])
def segmentation():
    result = run_segmentation()
    return jsonify(result)

#  Anomaly Detection 
@app.route("/anomaly/detect", methods=["POST"])
def anomaly():
    result = detect_anomalies()
    return jsonify(result)

@app.route("/anomaly/train", methods=["POST"])
def train_anomaly():
    result = train_anomaly_model()
    return jsonify(result)

if __name__ == "__main__":
    port = int(os.getenv("FLASK_PORT", 5100))
    app.run(host="0.0.0.0", port=port, debug=True)