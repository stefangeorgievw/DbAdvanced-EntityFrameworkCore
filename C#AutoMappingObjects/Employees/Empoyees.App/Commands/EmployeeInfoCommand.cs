using AutoMapper.QueryableExtensions;
using Employees.Data;
using Empoyees.App.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Empoyees.App.Commands
{
    class EmployeeInfoCommand : Command
    {
        private const string EmployeeNotFoundExceptionMessage = "Employee with Id {0} not found";

        private EmployeesContext context;
      

        public EmployeeInfoCommand(string[] cmdArgs, EmployeesContext context)
            : base(cmdArgs)
        {
            this.context = context;
           
        }
        
        public override void Execute()
        {
            if (this.CmdArgs.Length != 1)
            {
                throw new ArgumentException("The arguments count is not correct!");
            }

            var id = int.Parse(this.CmdArgs[0]);
            var employee = this.context.Employees
                .ProjectTo<EmployeeDto>()
                .SingleOrDefault(e => e.Id == id);

            if (employee == null)
            {
                throw new ArgumentException(string.Format(EmployeeNotFoundExceptionMessage, id));
            }

            Console.WriteLine(employee.ToString());
        }
    }
}
