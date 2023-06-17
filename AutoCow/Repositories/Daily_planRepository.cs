﻿using AutoCow.Models;
using Microsoft.Data.SqlClient;
using System.Collections.Specialized;
using System.Numerics;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Security.Cryptography.X509Certificates;
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
                            Console.WriteLine(daily_plan.date.ToString() + " -------------Milk: " + daily_plan.total + "------------Type: " + daily_plan.type);
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
                            Console.WriteLine(daily_plan.condition.ToString() + " -------------: " + daily_plan.count_condition);
                            daily_planList.Add(daily_plan);
                        }
                    }
                }
            }

            return daily_planList;
        }


        public Boolean Add(Daily_plan dailyPlan)
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
                            return false;
                        }
                    }

                }


                // Create the SQL query to insert values into the table
                string query = "INSERT INTO daily_plan (id, date, milk, temperature, disease, insemination, condition, weight, type, heart_rate, respiratory_rate) " +
                   "VALUES (@id, @date, @milk, @temperature, @disease, @insemination, @condition, 200, @type, @heart, @respiratory)";

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
                    command.Parameters.AddWithValue("@heart", dailyPlan.heart_rate);
                    command.Parameters.AddWithValue("@respiratory", dailyPlan.respiratory_rate);


                    // Execute the SQL query
                    command.ExecuteNonQuery();
                    Console.WriteLine("Executed....");
                }

                return true;


            }
        }


        public Boolean AddProduction(Daily_plan dailyPlan)
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

                                    return true;
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
                        return true;
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

        public Cow_profile UpdateCow_ProfileData(Daily_plan profile)
        {
            Cow_profile cow_Profile = new Cow_profile();
            cow_Profile.id = profile.id;
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT AVG(milk) AS average_milk, AVG(temperature) AS average_temp FROM daily_plan WHERE id = @id AND date >= DATEADD(DAY, -3, GETDATE())";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", profile.id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cow_Profile.avg_milk = (int)reader["average_milk"];
                            cow_Profile.avg_temperature = (int)reader["average_temp"];

                            reader.Close();
                            string insertQuery = "Update cow_profile set avg_milk = @avg_milk, avg_temp = @avg_temp where id = @cow_id";
                            using (SqlCommand insert = new SqlCommand(insertQuery, connection))
                            {

                                insert.Parameters.AddWithValue("@cow_id", cow_Profile.id);
                                insert.Parameters.AddWithValue("@avg_milk", cow_Profile.avg_milk);
                                insert.Parameters.AddWithValue("@avg_temp", cow_Profile.avg_temperature);

                                insert.ExecuteNonQuery();
                                Console.WriteLine("Insertion Executed...Avg.");

                            }

                        }

                        reader.Close();
                    }
                }

                return cow_Profile;
            }

        }

        public void deleteCowDailyData(Cow_profile cow_profile)
        {
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                String delete_query = "delete from daily_data where id = @id";

                using (SqlCommand command = new SqlCommand(delete_query, connection))
                {
                    command.Parameters.AddWithValue("@id", cow_profile.id);

                    command.ExecuteNonQuery();
                    Console.WriteLine("Cow with id " + cow_profile.id + " has been deleted... from daily data");
                }
            }
        }


        public List<Daily_plan> getLeaderboard()
        {
            List<Daily_plan> leaderboard = new List<Daily_plan>();
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                string query = "SELECT id, type, SUM(milk) AS total_milk FROM daily_plan WHERE date >= DATEADD(DAY, -7, GETDATE()) GROUP BY id, type Order by total_milk";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            Daily_plan data = new Daily_plan();
                            data.id = (int)reader["id"];
                            data.milk = (int)reader["total_milk"];
                            data.type = reader["type"].ToString();
                            Console.WriteLine("cow id = " + data.id + " type = "+ data.type +"  and milk = " + data.milk);
                            leaderboard.Add(data);
                        }

                        reader.Close();
                    }
                }

                return leaderboard;
            }
        }

    }
}

