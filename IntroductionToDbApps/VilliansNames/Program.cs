using System;
using System.Data.SqlClient;
using System.IO;

namespace VilliansNames
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = new SqlConnection(@"Server =.;Database=MinionsDB;Integrated Security = true");

            using (connection)
            {
                string query = File.ReadAllText(@"../../../FindVillainsCounts.sql");
                SqlCommand command = new SqlCommand(query, connection);
                SqlDataReader reader = command.ExecuteReader();

                using (reader)
                {
                    while (reader.Read())
                    {
                        string villainName = (string)reader["Name"];
                        int numberOfMinions = (int)reader["NumberOfMinions"];
                        Console.WriteLine($"{villainName} {numberOfMinions}");
                    }
                }
            }


        }
    }
}
