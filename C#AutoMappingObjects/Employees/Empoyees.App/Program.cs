using AutoMapper;
using Employees.Data;
using Employees.Models;
using Empoyees.App.Core;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Empoyees.App
{
    class Program
    {
        static void Main(string[] args)
        {
            InitializeMapper();
            using(var context = new EmployeesContext())
            {
                var engine = new Engine();
                engine.Run(context);
            }
            
        }

        public static void InitializeMapper()
        {
            Mapper.Initialize(cfg => cfg.AddProfile<MapperProfile>());
        }
    }
}
