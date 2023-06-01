using AutoCow.Models;
using Microsoft.Data.SqlClient;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AutoCow.Repositories
{
    public class Daily_planRepository
    {
        private readonly string _connectionString;

        public Daily_planRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Daily_plan> GetDailyData()
        {
            List<Daily_plan> daily_planList = new List<Daily_plan>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                DateTime endDate = DateTime.Now;
                DateTime startDate = endDate.AddDays(-7);
                string query = "SELECT date, type, SUM(milk) as total FROM daily_plan where date BETWEEN @startDate AND @endDate  GROUP BY date,type  ORDER BY date;";


                Console.WriteLine(query);
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Daily_plan daily_plan = new Daily_plan();
                            daily_plan.date = (DateTime)reader["date"];
                            daily_plan.total = (int)reader["total"];
                            daily_plan.type = reader["type"].ToString();
                            Console.WriteLine(daily_plan.date.ToString() + " -------------Milk: " + daily_plan.total +  "------------Type: "+daily_plan.type);
                            daily_planList.Add(daily_plan);
                        }
                    }
                }
            }

            return daily_planList;
        }

        public List<Daily_plan> GetConditionCount()
        {
            List<Daily_plan> daily_planList = new List<Daily_plan>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT condition, COUNT(*) as count FROM daily_plan where date = CAST(GETDATE() AS DATE)  GROUP BY condition;";


                Console.WriteLine(query);
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Daily_plan daily_plan = new Daily_plan();
                            daily_plan.count_condition = (int)reader["count"];
                            daily_plan.condition = reader["condition"].ToString();
                            Console.WriteLine(daily_plan.condition.ToString() + " -------------Milk: " + daily_plan.count_condition + "------------Type: " + daily_plan.type);
                            daily_planList.Add(daily_plan);
                        }
                    }
                }
            }

            return daily_planList;
        }


        public void Add(Daily_plan dailyPlan)
        {
            Console.WriteLine("Info recieved at repository");

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Open the database connection
                connection.Open();
                DateTime dt = DateTime.Now;

                string checkquery = "SELECT * from daily_plan where id = @ID AND date = CAST(GETDATE() AS DATE)";
                using (SqlCommand command = new SqlCommand(checkquery, connection))
                {
                    command.Parameters.AddWithValue("@ID", dailyPlan.id);

                    Console.WriteLine("Date = " + DateTime.Now);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("Date = " + reader["date"].ToString() + " -----------" + DateTime.Now);
                            Console.WriteLine("Same primary key entered");
                            return;
                        }
                    }

                }


                    // Create the SQL query to insert values into the table
                    string query = "INSERT INTO daily_plan (id, date, milk, temperature, disease, insemination, condition, weight, type) " +
                       "VALUES (@id, @date, @milk, @temperature, @disease, @insemination, @condition, 200, @type)";

                // Create a new SqlCommand using the query and the connection
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    // Set the parameter values using the actual data
                    command.Parameters.AddWithValue("@id", dailyPlan.id);
                    command.Parameters.AddWithValue("@date", DateTime.Now);
                    command.Parameters.AddWithValue("@milk", dailyPlan.milk);
                    command.Parameters.AddWithValue("@temperature", dailyPlan.temperature);
                    command.Parameters.AddWithValue("@disease", dailyPlan.disease);
                    command.Parameters.AddWithValue("@insemination", dailyPlan.insemination);
                    command.Parameters.AddWithValue("@condition", dailyPlan.condition);
                    command.Parameters.AddWithValue("@type", dailyPlan.type);


                    // Execute the SQL query
                    command.ExecuteNonQuery();
                    Console.WriteLine("Executed....");
                }




            }
        }


        public void AddProduction(Daily_plan dailyPlan)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                // Open the database connection
                connection.Open();
                DateTime dt = DateTime.Now;

                string checkquery = "SELECT * from production where date = CAST(GETDATE() AS DATE)";
                using (SqlCommand command = new SqlCommand(checkquery, connection))
                {

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        { 
                            reader.Close();
                            string update_query = "UPDATE production SET milk_production = milk_production + (SELECT milk FROM daily_plan WHERE date = CAST(GETDATE() AS DATE) and id = @ID ) WHERE date = CAST(GETDATE() AS DATE)";
                            using (SqlCommand update = new SqlCommand(update_query, connection))
                            {

                                update.Parameters.AddWithValue("@ID", dailyPlan.id);
                                // Execute the SQL query
                                int rowsAffected = update.ExecuteNonQuery();

                                if (rowsAffected > 0)
                                {
                                    Console.WriteLine("Updation successful");

                                    return;
                                }
                            }
                        }
                        reader.Close();
                    }

                        string first_entry = "INSERT INTO production (date, milk_production) VALUES(@date, @milk)";
                        using (SqlCommand insert = new SqlCommand(first_entry, connection))
                        {
              
                            insert.Parameters.AddWithValue("@date", DateTime.Now);
                            insert.Parameters.AddWithValue("@milk", dailyPlan.milk);

                            insert.ExecuteNonQuery();
                            Console.WriteLine("Insertion Executed....");
                            return;
                        }
                    

                }
            }
        }


        public string getTypeById(int id)
        {
            string type = "";

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT type FROM cow_profile where id = @id ;";

                Console.WriteLine(query);
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            type = reader["type"].ToString();
                        }

                        reader.Close();
                    }
                }
            }

            return type;
        }
    }
    
}
