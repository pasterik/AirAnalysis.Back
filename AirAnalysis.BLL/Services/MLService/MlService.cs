using AirAnalysis.BLL.Services.MLService.Model;
using Microsoft.ML;
using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OnnxRuntime = Microsoft.ML.OnnxRuntime;

namespace AirAnalysis.BLL.Services.MLService
{
    public class MlService
    {
        private readonly string _modelFolder;
        private readonly MLContext _mlContext;

        public MlService(string modelFolder = null)
        {
            _mlContext = new MLContext();
            _modelFolder = modelFolder ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "models");
        }

        // ------------------------
        // FORECAST MODEL (Timeseries with lags) - BY PHENOMEN ONLY
        // ------------------------
        public List<TimeseriesForecast> PredictTimeseries(
            List<float> lastValues,
            int phenomenId,
            DateTime lastDate,
            int steps = 7)
        {
            if (lastValues == null || lastValues.Count < 3)
                throw new ArgumentException("Provide at least 3 previous values for lag features.");

            string modelPath = Path.Combine(_modelFolder, "forecast", $"forecast_p{phenomenId}.onnx");
            if (!File.Exists(modelPath))
                throw new FileNotFoundException($"Forecast model not found: {modelPath}");

            var predictions = new List<TimeseriesForecast>();

            var inputWindow = new float[3];
            Array.Copy(lastValues.ToArray(), inputWindow, 3);

            string inputName = GetOnnxInputName(modelPath);
            string outputName = GetOnnxOutputName(modelPath);

            for (int i = 0; i < steps; i++)
            {
                var inputData = new List<ForecastInput>
                {
                    new ForecastInput {
                        Lag1 = inputWindow[0],
                        Lag2 = inputWindow[1],
                        Lag3 = inputWindow[2]
                    }
                };

                var inputML = _mlContext.Data.LoadFromEnumerable(inputData);

                var pipeline = _mlContext.Transforms.Concatenate(inputName, "Lag1", "Lag2", "Lag3")
                    .Append(_mlContext.Transforms.ApplyOnnxModel(
                        modelFile: modelPath,
                        outputColumnNames: new[] { outputName },
                        inputColumnNames: new[] { inputName }));

                var model = pipeline.Fit(inputML);
                var transformed = model.Transform(inputML);

                var cursor = transformed.GetRowCursor(transformed.Schema);
                var outputColumn = cursor.Schema[outputName];
                var getter = cursor.GetGetter<VBuffer<float>>(outputColumn);

                cursor.MoveNext();
                VBuffer<float> outputBuffer = default;
                getter(ref outputBuffer);

                float predictedValue = outputBuffer.GetValues()[0];

                if (float.IsNaN(predictedValue) || float.IsInfinity(predictedValue))
                {
                    throw new InvalidOperationException($"Invalid prediction at step {i + 1}");
                }

                predictions.Add(new TimeseriesForecast
                {
                    Date = lastDate.AddDays(i + 1),
                    PredictedValue = predictedValue
                });

                inputWindow[0] = inputWindow[1];
                inputWindow[1] = inputWindow[2];
                inputWindow[2] = predictedValue;
            }

            return predictions;
        }

        // ------------------------
        // IMPROVED ANOMALY DETECTION MODEL - BY PHENOMEN ONLY
        // ------------------------
        public bool IsAnomaly(double value, int phenomenId)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentException("Invalid value provided");

            string modelPath = Path.Combine(_modelFolder, "anomaly", $"anomaly_p{phenomenId}.onnx");
            if (!File.Exists(modelPath))
                throw new FileNotFoundException($"Anomaly model not found: {modelPath}");

            // Завантажуємо метадані для розрахунку z-scores
            string metadataPath = Path.Combine(_modelFolder, "metadata", $"metadata_p{phenomenId}.txt");
            double mean = 0;
            double std = 1;

            if (File.Exists(metadataPath))
            {
                var lines = File.ReadAllLines(metadataPath);
                foreach (var line in lines)
                {
                    if (line.StartsWith("mean="))
                        mean = double.Parse(line.Split('=')[1], System.Globalization.CultureInfo.InvariantCulture);
                    else if (line.StartsWith("std="))
                        std = double.Parse(line.Split('=')[1], System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            // Розраховуємо z-scores
            float zScore = std > 0 ? (float)((value - mean) / std) : 0;
            float localZScore = zScore;

            var data = new List<ImprovedAnomalyInput>
            {
                new ImprovedAnomalyInput
                {
                    Value = (float)value,
                    ZScore = Math.Abs(zScore),
                    LocalZScore = Math.Abs(localZScore)
                }
            };

            var inputData = _mlContext.Data.LoadFromEnumerable(data);

            string inputName = GetOnnxInputName(modelPath);
            string outputName = GetOnnxOutputName(modelPath);

            var pipeline = _mlContext.Transforms.Concatenate(inputName, "Value", "ZScore", "LocalZScore")
                .Append(_mlContext.Transforms.ApplyOnnxModel(
                    modelFile: modelPath,
                    outputColumnNames: new[] { outputName },
                    inputColumnNames: new[] { inputName }));

            var model = pipeline.Fit(inputData);
            var transformed = model.Transform(inputData);

            var cursor = transformed.GetRowCursor(transformed.Schema);
            var outputColumn = cursor.Schema[outputName];
            var getter = cursor.GetGetter<VBuffer<long>>(outputColumn);

            cursor.MoveNext();
            VBuffer<long> outputBuffer = default;
            getter(ref outputBuffer);

            long result = outputBuffer.GetValues()[0];

            return result >= 1;
        }

        // ------------------------
        // ANOMALY DETECTION WITH METADATA - BY PHENOMEN ONLY
        // ------------------------
        public AnomalyResult CheckAnomalyWithMetadata(double value, int phenomenId)
        {
            var isAnomaly = IsAnomaly(value, phenomenId);

            string metadataPath = Path.Combine(_modelFolder, "metadata", $"metadata_p{phenomenId}.txt");

            double? mean = null;
            double? std = null;

            if (File.Exists(metadataPath))
            {
                var lines = File.ReadAllLines(metadataPath);
                foreach (var line in lines)
                {
                    if (line.StartsWith("mean="))
                        mean = double.Parse(line.Split('=')[1], System.Globalization.CultureInfo.InvariantCulture);
                    else if (line.StartsWith("std="))
                        std = double.Parse(line.Split('=')[1], System.Globalization.CultureInfo.InvariantCulture);
                }
            }

            return new AnomalyResult
            {
                Value = value,
                IsAnomaly = isAnomaly,
                Mean = mean,
                Std = std,
                DeviationFromMean = mean.HasValue ? Math.Abs(value - mean.Value) : null,
                ZScore = mean.HasValue && std.HasValue && std.Value > 0
                    ? Math.Abs((value - mean.Value) / std.Value)
                    : null
            };
        }

        // ------------------------
        // BATCH ANOMALY DETECTION
        // ------------------------
        public List<AnomalyResult> CheckAnomaliesBatch(List<double> values, int phenomenId)
        {
            var results = new List<AnomalyResult>();

            foreach (var value in values)
            {
                try
                {
                    results.Add(CheckAnomalyWithMetadata(value, phenomenId));
                }
                catch (Exception ex)
                {
                    results.Add(new AnomalyResult
                    {
                        Value = value,
                        IsAnomaly = false,
                        Error = ex.Message
                    });
                }
            }

            return results;
        }

        // ------------------------
        // MODEL AVAILABILITY CHECK - BY PHENOMEN ONLY
        // ------------------------
        public bool IsForecastModelAvailable(int phenomenId)
        {
            string modelPath = Path.Combine(_modelFolder, "forecast", $"forecast_p{phenomenId}.onnx");
            return File.Exists(modelPath);
        }

        public bool IsAnomalyModelAvailable(int phenomenId)
        {
            string modelPath = Path.Combine(_modelFolder, "anomaly", $"anomaly_p{phenomenId}.onnx");
            return File.Exists(modelPath);
        }

        public bool IsClassificationModelAvailable()
        {
            string modelPath = Path.Combine(_modelFolder, "classification", "global_classifier.onnx");
            return File.Exists(modelPath);
        }

        // ------------------------
        // GET AVAILABLE MODELS INFO
        // ------------------------
        public ModelAvailabilityInfo GetModelAvailability()
        {
            var forecastDir = Path.Combine(_modelFolder, "forecast");
            var anomalyDir = Path.Combine(_modelFolder, "anomaly");

            var availableForecast = new List<int>();
            var availableAnomaly = new List<int>();

            if (Directory.Exists(forecastDir))
            {
                var forecastFiles = Directory.GetFiles(forecastDir, "forecast_p*.onnx");
                foreach (var file in forecastFiles)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var parts = fileName.Split('_');
                    if (parts.Length >= 2 && parts[1].StartsWith("p"))
                    {
                        if (int.TryParse(parts[1].Substring(1), out int phenomenId))
                        {
                            availableForecast.Add(phenomenId);
                        }
                    }
                }
            }

            if (Directory.Exists(anomalyDir))
            {
                var anomalyFiles = Directory.GetFiles(anomalyDir, "anomaly_p*.onnx");
                foreach (var file in anomalyFiles)
                {
                    var fileName = Path.GetFileNameWithoutExtension(file);
                    var parts = fileName.Split('_');
                    if (parts.Length >= 2 && parts[1].StartsWith("p"))
                    {
                        if (int.TryParse(parts[1].Substring(1), out int phenomenId))
                        {
                            availableAnomaly.Add(phenomenId);
                        }
                    }
                }
            }

            return new ModelAvailabilityInfo
            {
                AvailableForecastModels = availableForecast.Distinct().OrderBy(x => x).ToList(),
                AvailableAnomalyModels = availableAnomaly.Distinct().OrderBy(x => x).ToList(),
                HasClassificationModel = IsClassificationModelAvailable()
            };
        }

        // ------------------------
        // HELPERS
        // ------------------------
        private string GetOnnxInputName(string modelPath)
        {
            using var session = new OnnxRuntime.InferenceSession(modelPath);
            return session.InputMetadata.Keys.First();
        }

        private string GetOnnxOutputName(string modelPath)
        {
            using var session = new OnnxRuntime.InferenceSession(modelPath);
            return session.OutputMetadata.Keys.First();
        }
    }
}