using Microsoft.ML;
using AutoCow.Models;
using Microsoft.Data.SqlClient;
using Microsoft.ML.Data;

namespace AutoCow.Repositories
{
    public class MilkProductionPrediction
    {
        [ColumnName("Score")]
        public int PredictedMilkProduction { get; set; }
    }
    public class MachineLearning
    {

        private readonly string _connectionString;

        public MachineLearning(string connectionString)
        {
            _connectionString = connectionString;
        }
        public void ML()
        {

            // Connection string to the database
            string connectionString = _connectionString;

            // Load the data from the database table
            List<ProductionData> data = LoadDataFromDatabase(connectionString);

            // Create a new MLContext
            var mlContext = new MLContext();

            // Convert the data to an IDataView
            var dataView = mlContext.Data.LoadFromEnumerable(data);

            // Split the data into training and testing sets
            var trainTestData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);

            // Define the pipeline
            var pipeline = mlContext.Transforms.Concatenate("Features", "MilkProduction")
                .Append(mlContext.Transforms.CopyColumns("Label", "MilkProduction"))
                .Append(mlContext.Transforms.NormalizeMinMax("Features"))
                .Append(mlContext.Regression.Trainers.Sdca())
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedMilkProduction"));
            // Train the model
            var model = pipeline.Fit(trainTestData.TrainSet);

            // Make predictions on the test set
            var predictions = model.Transform(trainTestData.TestSet);

            // Evaluate the model
            var metrics = mlContext.Regression.Evaluate(predictions);

            Console.WriteLine($"R2 Score: {metrics.RSquared}");

            // Create a new prediction engine
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ProductionData, MilkProductionPrediction>(model);

            // Get the feature data for tomorrow's date from the database
            DateTime tomorrow = DateTime.Today.AddDays(1);

            // Create a new instance of MilkProductionData with the feature data for tomorrow
            var tomorrowData = new ProductionData
            {
                Date = tomorrow,
                MilkProduction = 0
            };

            // Make a prediction for tomorrow
            var prediction = predictionEngine.Predict(tomorrowData);

            Console.WriteLine($"Predicted milk production for tomorrow ({tomorrow.ToShortDateString()}): {prediction.PredictedMilkProduction}");
        }

        static List<ProductionData> LoadDataFromDatabase(string connectionString)
        {
            var data = new List<ProductionData>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                string query = "SELECT date, milk_production FROM production ORDER BY date";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var date = (DateTime)reader["date"];
                            var milkProduction = (int)reader["milk_production"];

                            var milkProductionData = new ProductionData
                            {
                                Date = date,
                                MilkProduction = milkProduction
                            };

                            data.Add(milkProductionData);
                        }
                    }
                }
            }

            return data;
        }

    }
}
