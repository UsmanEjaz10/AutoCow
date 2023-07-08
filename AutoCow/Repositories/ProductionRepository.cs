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
                string query = "SELECT date, milk_production FROM Production where date BETWEEN @startDate AND @endDate order by date";

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


        // Returns the average daily milk produced by the cows. It helps to identify whether the milk produced today was higher or lower than the lasts ones //
        public int averageProduction()
        {
            int avg = 0;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {

                connection.Open();
                DateTime endDate = DateTime.Now;
                DateTime startDate = endDate.AddDays(-7);
                string query = "select AVG(milk_production) as average_milk FROM production where date BETWEEN @startDate AND @endDate";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            avg = (int)reader["average_milk"];
                        }
                        reader.Close();
                    }
                }
            }
            return avg;
        }


		public List<ProductionData> GetAllTimeProductionData()
		{
			List<ProductionData> productionDataList = new List<ProductionData>();

			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				string query = "SELECT date, milk_production FROM Production where date >= DATEADD(DAY, -10, GETDATE())";

				using (SqlCommand command = new SqlCommand(query, connection))
				{
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
							ProductionData productionData = new ProductionData();
							productionData.Date = (DateTime)reader["date"];
							productionData.MilkProduction = (int)reader["milk_production"];
							productionDataList.Add(productionData);
						}
					}
				}
			}

			return productionDataList;
		}


        public List<int> GetWeekWiseData()
        {
            List<int> milk = new List<int>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "";
                
                     query = "SELECT DATEADD(WEEK, DATEDIFF(WEEK, 0, date), 0) AS week_start_date, SUM(milk_production) AS milk_sum FROM production WHERE date>= DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()) - 1, 0)  AND date < DATEADD(MONTH, DATEDIFF(MONTH, 0, GETDATE()), 0)  GROUP BY DATEADD(WEEK, DATEDIFF(WEEK, 0, date), 0) ORDER BY week_start_date;";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                           int  milk_sum = (int)reader["milk_sum"];
                            milk.Add(milk_sum);
                        }
                    }
                }
            }

            return milk;
        }
    }

}
