using AutoMapper;
using Employees.Data;
using Employees.Models;
using Empoyees.App.Commands.Interfaces;
using Empoyees.App.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Empoyees.App.Commands
{
    class AddEmployeeCommand : Command
    {
        private EmployeesContext context;

        public AddEmployeeCommand(string[] cmdArgs, EmployeesContext context):base(cmdArgs)
        {
            
            this.context = context;
        }

        public override void Execute()
        {
            if (this.CmdArgs.Length != 3)
            {
                throw new InvalidOperationException("The arguments count is not correct!");
            }

            var firstName = this.CmdArgs[0];
            var lastName = this.CmdArgs[1];
            var salary = decimal.Parse(this.CmdArgs[2]);

            var dto = new EmployeeDto(firstName, lastName, salary);
            var employee = Mapper.Map<Employee>(dto);

            this.context.Employees.Add(employee);
            this.context.SaveChanges();
        }
    }
}
