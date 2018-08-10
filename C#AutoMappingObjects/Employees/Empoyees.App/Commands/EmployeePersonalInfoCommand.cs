using Employees.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace Empoyees.App.Commands
{
    class EmployeePersonalInfoCommand : Command
    {
        private const string EmployeeNotFoundExceptionMessage = "Employee with Id {0} not found";
        private EmployeesContext context;

        public EmployeePersonalInfoCommand(string[] cmdArgs, EmployeesContext context ) : base(cmdArgs)
        {
            this.context = context;
        }

        public override void Execute()
        {
            if (this.CmdArgs.Length != 1)
            {
                throw new ArgumentException(InvalidCommandArgumentsExceptionMessage);
            }

            var id = int.Parse(this.CmdArgs[0]);
            var employee = this.context.Employees.Find(id);
            if (employee == null)
            {
                throw new ArgumentException(string.Format(EmployeeNotFoundExceptionMessage, id));
            }

            Console.WriteLine(employee.ToString());
        }
    }
}
