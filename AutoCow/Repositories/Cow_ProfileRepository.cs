using AutoCow.Models;
using Microsoft.Data.SqlClient;

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
    }
}
