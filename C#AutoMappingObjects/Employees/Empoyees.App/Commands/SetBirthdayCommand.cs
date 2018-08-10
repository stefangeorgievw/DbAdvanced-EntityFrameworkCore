using Employees.Data;
using Employees.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Empoyees.App.Commands
{
    class SetBirthdayCommand : Command
    {
        private const string DateFormat = "dd-MM-yyyy";

        private EmployeesContext context;

        public SetBirthdayCommand(string[] cmdArgs,EmployeesContext context) : base(cmdArgs)
        {
            this.context = context;
        }

        public override void Execute()
        {
            var employeeId = int.Parse(this.CmdArgs[0]);
            var dateString = this.CmdArgs[1];

            var employee = this.GetEmployee(employeeId);
            var date = this.TryParseDate(dateString);

            employee.Birthday = date;
            context.SaveChanges();


        }

        private DateTime TryParseDate(string dateString)
        {
            DateTime date;
            var isparsed = DateTime.TryParseExact(dateString, DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out date);
            if (isparsed)
            {
                return date;
            }

            throw new ArgumentException();
        }

        private Employee GetEmployee(int employeeId)
        {
            var employee = this.context.Employees
                .SingleOrDefault(e => e.Id == employeeId);

            if (employee == null)
            {
                throw new AggregateException(string.Format($"There is no employee with {employeeId}id!"));
            }

            return employee;
        }
    }
}
