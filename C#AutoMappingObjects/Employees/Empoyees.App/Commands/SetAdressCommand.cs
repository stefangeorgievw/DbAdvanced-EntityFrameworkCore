using Employees.Data;
using Employees.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Empoyees.App.Commands
{
    class SetAdressCommand : Command
    {
        
        private EmployeesContext context;

        public SetAdressCommand(string[] cmdArgs, EmployeesContext context) : base(cmdArgs)
        {
            this.context = context;
        }

        public override void Execute()
        {
            if (this.CmdArgs.Length < 2)
            {
                throw new AggregateException("Arguments count is not correct");
            }

            var employeeId = int.Parse(this.CmdArgs[0]);

            var employee = this.GetEmployee(employeeId);
            var address = string.Join(" ", this.CmdArgs.Skip(1));

            employee.Address = address;
            context.SaveChanges();
        }

        private Employee GetEmployee(int employeeId)
        {
            var employee = this.context.Employees
                .SingleOrDefault(e => e.Id == employeeId);

            if (employee == null)
            {
                throw new AggregateException($"There is no employee with {employeeId} id");
            }

            return employee;
        }
    }
}
