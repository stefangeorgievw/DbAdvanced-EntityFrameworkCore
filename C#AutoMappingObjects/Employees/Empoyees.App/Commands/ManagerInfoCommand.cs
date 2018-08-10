using AutoMapper.QueryableExtensions;
using Employees.Data;
using Empoyees.App.DTOs;
using System;
using System.Linq;


namespace Empoyees.App.Commands
{
    class ManagerInfoCommand:Command
    {
        private const string EmployeeNotFoundExceptionMessage = "Manager with Id {0} not found";

        private EmployeesContext context;
       

        public ManagerInfoCommand(string[] cmdArgs, EmployeesContext context)
            : base(cmdArgs)
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
            var manager = this.context.Employees
                .ProjectTo<ManagerDto>()
                .SingleOrDefault(e => e.Id == id);

            if (manager == null)
            {
                throw new ArgumentException(string.Format(EmployeeNotFoundExceptionMessage, id));
            }

            Console.WriteLine(manager.ToString());
        }
    }
}
