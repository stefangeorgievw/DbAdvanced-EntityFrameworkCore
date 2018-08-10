using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.Data
{
    public static class ConnectionConfig
    {
        public static string ConnectionString => "Server=(LocalDB)\\MSSQLLocalDB;Database=CarDealer; Trusted_Connection=True;";
    }
}
