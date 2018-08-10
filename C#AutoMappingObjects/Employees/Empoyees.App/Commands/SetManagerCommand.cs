using Employees.Data;
using Employees.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Empoyees.App.Commands
{
    class SetManagerCommand:Command
    {
        private EmployeesContext context;

        public SetManagerCommand(string[] cmdArgs, EmployeesContext context)
            : base(cmdArgs)
        {
            this.context = context;
        }

        // SetManager <employeeId> <managerId> 
        public override void Execute()
        {
            if (this.CmdArgs.Length != 2)
            {
                throw new ArgumentException(InvalidCommandArgumentsExceptionMessage);
            }

            var employee = this.GetEmployee(int.Parse(CmdArgs[0]));
            var manager = this.GetEmployee(int.Parse(this.CmdArgs[1]));

            employee.Manager = manager;
            this.context.SaveChanges();
        }

        private Employee GetEmployee(int id)
        {
            var employee = this.context.Employees
                .SingleOrDefault(e => e.Id == id);

            if (employee == null)
            {
                throw new ArgumentException(InvalidCommandArgumentsExceptionMessage);
            }

            return employee;
        }
    }
}
