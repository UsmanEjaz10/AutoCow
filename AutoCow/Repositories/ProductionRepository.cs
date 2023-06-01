using AutoCow.Models;
using Microsoft.Data.SqlClient;

namespace AutoCow.Repositories
{
    public class ProductionRepository
    {
        private readonly string _connectionString;

        public ProductionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<ProductionData> GetProductionData()
        {
            List<ProductionData> productionDataList = new List<ProductionData>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                DateTime endDate = DateTime.Now;
                DateTime startDate = endDate.AddDays(-7);
                string query = "SELECT date, milk_production FROM Production where date BETWEEN @startDate AND @endDate";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProductionData productionData = new ProductionData();
                            productionData.Date = (DateTime)reader["date"];
                            productionData.MilkProduction = (int)reader["milk_production"];
                            Console.WriteLine(productionData.Date.ToString() + " " + productionData.MilkProduction);
                            productionDataList.Add(productionData);
                        }
                    }
                }
            }

            return productionDataList;
        }
    }

}
