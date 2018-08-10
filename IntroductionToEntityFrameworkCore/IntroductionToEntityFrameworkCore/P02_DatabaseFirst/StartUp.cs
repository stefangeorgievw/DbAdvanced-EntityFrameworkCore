using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using P02_DatabaseFirst.Data;
using P02_DatabaseFirst.Data.Models;

namespace P02_DatabaseFirst
{
    class StartUp
    {
        static void Main(string[] args)
        {
            using(var db = new SoftUniContext())
            {
                SoftUniContext context = new SoftUniContext();

               // P03_EmployeesFullInformation(context);
               // P04_EmployeesWithSalaryOver50000(context);
               // P05_EmployeesFromRD(context);
               // P06_AddNewAddressUpdateEmployee(context);
               // P07_FindEmployeesInPeriod(context);
              //  P08_AddressesByTownName(context);
               // P09_EmployeeId147(context);
                //P10_DepartmentsWithMoreThan5Employees(context);
               // P11_FindLatest10Projects(context);
                //P12_IncreaseSalaries(context);
               // P13_FindEmployeesByFirstNameStartingWithSA(context);
                P15_DeleteProjectById(context);

            }

        }

        private static void P15_DeleteProjectById(SoftUniContext context)
        {
            var project = context.Projects.First(p => p.ProjectId == 2);

            context.EmployeesProjects.ToList().ForEach(ep => context.EmployeesProjects.Remove(ep));
            context.Projects.Remove(project);

            context.SaveChanges();

            context.Projects.Take(10).Select(p => p.Name).ToList().ForEach(p => Console.WriteLine(p));
        }

        private static void P13_FindEmployeesByFirstNameStartingWithSA(SoftUniContext context)
        {
            context.Employees
                    .Where(e => e.FirstName.Substring(0, 2) == "Sa")
                    .Select(e => new { e.FirstName, e.LastName, e.JobTitle, e.Salary })
                    .OrderBy(e => e.FirstName)
                    .ThenBy(e => e.LastName)
                    .ToList()
.ForEach(e => Console.WriteLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})"));
        }

        private static void P12_IncreaseSalaries(SoftUniContext context)
        {
            context.Employees
                    .Where(e => new[] { "Engineering", "Tool Design", "Marketing", "Information Services" }
                        .Contains(e.Department.Name))
                    .ToList()
                    .ForEach(e => e.Salary *= 1.12m);

            context.SaveChanges();

            context.Employees
                .Where(e => new[] { "Engineering", "Tool Design", "Marketing", "Information Services" }
                    .Contains(e.Department.Name))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToList()
.ForEach(e => Console.WriteLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})"));
        }

        private static void P11_FindLatest10Projects(SoftUniContext context)
        {
            context.Projects.
       OrderByDescending(p => p.StartDate).
       Take(10).
       Select(p => new { p.Name, p.Description, p.StartDate })
       .OrderBy(p => p.Name)
       .ToList()
.ForEach(p => Console.WriteLine($"{p.Name}{Environment.NewLine}{p.Description}{Environment.NewLine}{p.StartDate}"));

        }

        private static void P10_DepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            context.Departments
                     .Include(d => d.Employees)
                     .Include(d => d.Manager)
                     .Where(d => d.Employees.Count > 5)
                     .OrderBy(d => d.Employees.Count)
                     .ThenBy(d => d.Name)
                     .ToList()
 .ForEach(d => Console.WriteLine($"{d.Name} - {d.Manager.FirstName} {d.Manager.LastName}{Environment.NewLine}{String.Join(Environment.NewLine, d.Employees.OrderBy(e => e.FirstName).ThenBy(e => e.LastName).Select(e => $"{e.FirstName} {e.LastName} - {e.JobTitle}").ToList())}{Environment.NewLine}{new string('-', 10)}"));
        }

        private static void P09_EmployeeId147(SoftUniContext context)
        {
            var employee = context.Employees.
                    Where(e => e.EmployeeId == 147)
                    .Select(e => new {
                        e.FirstName,
                        e.LastName,
                        e.JobTitle,
                        Projects = e.EmployeesProjects
                            .Select(ep => ep.Project.Name)
                            .OrderBy(p => p)
                            .ToList()
                    })
                    .First();

            Console.WriteLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}{Environment.NewLine}{String.Join(Environment.NewLine, employee.Projects)}");
        }

        private static void P08_AddressesByTownName(SoftUniContext context)
        {
            var adresses = context.Addresses
                .OrderByDescending(x => x.Employees.Count)
                .ThenBy(x=> x.Town.Name)
                .ThenBy(x=> x.AddressText)
                 .Select(x => new
                 {
                     x.AddressText,
                     TownName = x.Town.Name,
                     countOfEmployees = x.Employees.Count
                 }
                 )
                 .Take(10)
                 .ToArray();

            foreach (var a in adresses)
            {
                Console.WriteLine($"{a.AddressText}, {a.TownName} - {a.countOfEmployees}");
            }
        }

        private static void P07_FindEmployeesInPeriod(SoftUniContext context)
        {
           context.Employees
                   .Where(e => e.EmployeesProjects
                       .Any(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003))
                   .Take(30)
                   .Select(e => new {
                       e.FirstName,
                       e.LastName,
                       ManagerFirstName = e.Manager.FirstName,
                       ManagerLastName = e.Manager.LastName,
                       Projects = e.EmployeesProjects
                           .Select(ep => ep.Project)
                   })
                   .ToList()
                   .ForEach(e => Console.WriteLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}{Environment.NewLine}" +
$"{String.Join(Environment.NewLine, e.Projects.Select(p => $"--{p.Name} - {p.StartDate.ToString()} - {(p.EndDate == null ? "not finished" : p.EndDate.ToString())}"))}"));

        }

        private static void P06_AddNewAddressUpdateEmployee(SoftUniContext context)
        {
            var adress = new Address() { AddressText = "Vitoshka 15", TownId = 4 };

            var nakov = context.Employees.FirstOrDefault(x => x.LastName == "Nakov");

            nakov.Address = adress;
            context.SaveChanges();

            var employees = context.Employees.OrderByDescending(x => x.AddressId).Take(10).Select(a => a.Address.AddressText).ToList();

            Console.WriteLine(string.Join("\n", employees));
        }

        private static void P05_EmployeesFromRD(SoftUniContext context)
        {
            context.Employees
                    .Where(e => e.Department.Name == "Research and Development")
                    .OrderBy(e => e.Salary).
                    ThenByDescending(e => e.FirstName)
                    .Select(e => new { e.FirstName, e.LastName, DepartmentName = e.Department.Name, e.Salary })
                    .ToList()
.ForEach(e => Console.WriteLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:f2}"));

            


        }

        private static void P04_EmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var employees = context.Employees
                .OrderBy(e=> e.FirstName)
                .Select(x=> new {x.FirstName,x.Salary})
                .Where(x=> x.Salary> 50000).ToList();
            foreach (var e in employees)
            {
                Console.WriteLine(e.FirstName);
            }
        }

        private static void P03_EmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees.ToList();
            foreach (var e in employees.OrderBy(e=> e.EmployeeId))
            {
                Console.WriteLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:F2}");
            }
        }
    
}
}
