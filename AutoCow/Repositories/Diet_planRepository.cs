using AutoCow.Models;
using Microsoft.Data.SqlClient;

namespace AutoCow.Repositories
{
    public class Diet_planRepository
    {

        private readonly string _connectionString;

        public Diet_planRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public Diet_plan getAllEatables(String category)
        {
            Diet_plan dp = new Diet_plan();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "Select * from diet_plan where category = @category;";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@category", category);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            dp.category = category;
                            dp.eatable1 = reader["eatable1"].ToString();
                            dp.eatable2 = reader["eatable2"].ToString();
                            dp.eatable3 = reader["eatable3"].ToString();

                        }
                    }
                }

            }
            return dp;

        }

    }
}
