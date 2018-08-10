using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.Data.Config
{
    class Configuration
    {
        public static string ConnectionString => @"Server=(LocalDB)\MSSQLLocalDB;Database=Product Shop; Trusted_Connection=True;";
    }
}
