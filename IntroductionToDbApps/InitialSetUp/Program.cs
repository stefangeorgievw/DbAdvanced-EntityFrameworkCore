using System;
using System.Data.SqlClient;
using System.IO;

namespace InitialSetUp
{
    class Program
    {
        static void Main(string[] args)
        {
            var connection = new SqlConnection(@"Server=.;Integrated Security=true");
            connection.Open();

            using (connection)
            {
              
                CreateDb(connection);
            }
        }

        private static void CreateDb(SqlConnection connection)
        {
            string query = File.ReadAllText(@"../../../MinionsDB.sql");
            SqlCommand command = new SqlCommand(query, connection);
            Console.WriteLine("Created DB tables. Rows affected {0}.", command.ExecuteNonQuery());
        }


       
       
    }
}
