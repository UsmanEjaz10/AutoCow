using AutoCow.Models;
using Microsoft.Data.SqlClient;

namespace AutoCow.Repositories
{
    public class Monthly_ProductionRepository
    {
        private readonly string _connectionString;

        public Monthly_ProductionRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

    

    public float GetThisMonthData()
    {
        float milk = 0;

        using (SqlConnection connection = new SqlConnection(_connectionString))
        {
            connection.Open();
            string query = "select SUM(milk_production) as milk from production where  MONTH(date) = Month(getdate())";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        milk = (int)reader["milk"];
                        Console.WriteLine("This month's production = " + milk);
                    }
                }
            }
        }


        return milk;
    }

    }
}