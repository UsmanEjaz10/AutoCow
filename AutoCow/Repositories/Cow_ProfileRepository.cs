﻿using AutoCow.Models;
using Microsoft.Data.SqlClient;
using System.IO.Pipelines;

namespace AutoCow.Repositories
{
    public class Cow_ProfileRepository
    {
        private readonly string _connectionString;

        public Cow_ProfileRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<Cow_profile> GetAllCows()
        {
            List<Cow_profile> cow_profileList = new List<Cow_profile>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT * FROM cow_profile";


                Console.WriteLine(query);
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Cow_profile cow_profile = new Cow_profile();
                            cow_profile.id = (int)reader["id"];
                            cow_profile.type = reader["type"].ToString();
                            cow_profile.price = (int)reader["price"];
                            cow_profile.color = reader["color"].ToString();
                            cow_profile.avg_milk = (int)reader["avg_milk"];
                            cow_profile.avg_temperature = (int)reader["avg_temp"];
                            cow_profile.category = reader["category"].ToString();

                            //    cow_profile.avg_milk = (int)reader["avg_milk"];
                            //    cow_profile.avg_temperature = (int)reader["avg_temp"];
                            //    cow_profile.category = reader["category"].ToString();
                            Console.WriteLine(cow_profile.id + " " + cow_profile.type + "  of price = " + cow_profile.price + " entered into cow_profile db");
                            cow_profileList.Add(cow_profile);
                        }
                    }
                }
            }

            return cow_profileList;
        }

        public void AddCow(Cow_profile cow_profile)
        {

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                String checkquery = "Select  * from cow_profile where id = @id";
                using (SqlCommand command = new SqlCommand(checkquery, connection))
                {
                    command.Parameters.AddWithValue("@id", cow_profile.id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Console.WriteLine("Date = " + reader["id"].ToString() + " -----------");
                            Console.WriteLine("Same primary key entered");
                            return;
                        }
                    }
                }

                string query = "Insert into cow_profile (id, color, type, price, avg_temp, avg_milk, category, dob) values(@id, @color, @type, @price, @temp, @milk, @category, @dob)";
                using (SqlCommand add = new SqlCommand(query, connection))
                {
                    add.Parameters.AddWithValue("@id", cow_profile.id);
                    add.Parameters.AddWithValue("@color", cow_profile.color);
                    add.Parameters.AddWithValue("@type", cow_profile.type);
                    add.Parameters.AddWithValue("@price", cow_profile.price);
                    add.Parameters.AddWithValue("@temp", cow_profile.avg_temperature);
                    add.Parameters.AddWithValue("@milk", cow_profile.avg_milk);
                    add.Parameters.AddWithValue("@category", cow_profile.category);
                    add.Parameters.AddWithValue("@dob", cow_profile.dob);


                    add.ExecuteNonQuery();
                    Console.WriteLine("New Cow Added...");

                }
            }
            
        }


        public void deleteCow(int id) {

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                String delete_query = "delete from cow_profile where id = @id";

                using (SqlCommand command = new SqlCommand(delete_query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);

                    command.ExecuteNonQuery();
                    Console.WriteLine("Cow with id " + id + " has been deleted...");
                }
            }

        }


        public Cow_profile GetCowById(int cow_id)
        {
            Cow_profile cow_profile = new Cow_profile();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                string query = "SELECT id, type, price, color, avg_milk, avg_temp, category, dob, datediff(YY, dob, getdate()) as age FROM cow_profile where id= @cow_id";


                Console.WriteLine(query);
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@cow_id", cow_id);
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cow_profile.id = (int)reader["id"];
                            cow_profile.type = reader["type"].ToString();
                            cow_profile.price = (int)reader["price"];
                            cow_profile.color = reader["color"].ToString();
                            cow_profile.avg_milk = (int)reader["avg_milk"];
                            cow_profile.avg_temperature = (int)reader["avg_temp"];
                            cow_profile.category = reader["category"].ToString();
                            cow_profile.dob = (DateTime)reader["dob"];
                            cow_profile.age = (int)reader["age"];

                            
                            //    cow_profile.avg_milk = (int)reader["avg_milk"];
                            //    cow_profile.avg_temperature = (int)reader["avg_temp"];
                            //    cow_profile.category = reader["category"].ToString();
                            Console.WriteLine(cow_profile.id + " " + cow_profile.type + "  of price = " + cow_profile.price + " from cow_profile db");
                            
                        }
                    }
                }
            }

            return cow_profile;
        }

        public int getCount(string ty) {

            int count = 0;
			using (SqlConnection connection = new SqlConnection(_connectionString))
			{
				connection.Open();
				String checkquery = "Select  COUNT(type) as NB from cow_profile where type = @t ";
				using (SqlCommand command = new SqlCommand(checkquery, connection))
				{
                    command.Parameters.AddWithValue("@t", ty);
					using (SqlDataReader reader = command.ExecuteReader())
					{
						while (reader.Read())
						{
                            count += (int)reader["NB"];
						}
					}
				}

			}

            return count;

        }
        
    }
}
