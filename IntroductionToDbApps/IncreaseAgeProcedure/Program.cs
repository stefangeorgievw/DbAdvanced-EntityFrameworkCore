using System;
using System.IO;
using System.Data.SqlClient;

namespace IncreaseAgeProcedure
{
    class Program
    {
        static void Main(string[] args)
        {
            SqlConnection connection = new SqlConnection("Server=.;Database=MinionsDB;Integrated security=true");
            connection.Open();

            using (connection)
            {
                
                CreateStoredProcToIncreaseAge(connection); 
 
                int minionId = int.Parse(Console.ReadLine());

                IncreaseAgeWithProc(connection, minionId);
                PrintMinionsNameAge(connection, minionId);
            }
        }



        private static void PrintMinionsNameAge(SqlConnection connection, int minionId)
        {
            string query = @"SELECT Name, Age FROM Minions WHERE Id = @id";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@id", minionId);
            SqlDataReader reader = command.ExecuteReader();

            using (reader)
            {
                while (reader.Read())
                {
                    string name = (string)reader["Name"];
                    int age = (int)reader["Age"];
                    Console.WriteLine($"{name} {age}");
                }
            }
        }

        private static void IncreaseAgeWithProc(SqlConnection connection, int minionId)
        {
            string execProc = @"EXEC usp_GetOlder @id";
            SqlCommand command = new SqlCommand(execProc, connection);
            command.Parameters.AddWithValue("@id", minionId);
            command.ExecuteNonQuery();
        }

        private static void CreateStoredProcToIncreaseAge(SqlConnection connection)
        {
            string storedProc = File.ReadAllText(@"../../../usp_GetOlder.sql");
            SqlCommand storedProcCmd = new SqlCommand(storedProc, connection);
            storedProcCmd.ExecuteNonQuery();
            Console.WriteLine("Created a store procedure usp_GetOlder");
        }
    }
}
