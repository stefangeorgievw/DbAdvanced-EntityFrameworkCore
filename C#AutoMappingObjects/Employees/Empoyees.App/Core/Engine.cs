using Employees.Data;
using Empoyees.App.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Empoyees.App.Core
{
    class Engine
    {
        public void Run(EmployeesContext context)
        {
            while (true)
            {
                var input = Console.ReadLine().Split(" ", StringSplitOptions.RemoveEmptyEntries);
                var commandString = input[0];
                var cmdAtr = input.Skip(1).ToArray();
                
                switch (commandString)
                {
                    
                    case "AddEmployee":
                        var command = new  AddEmployeeCommand(cmdAtr,context);
                        command.Execute();
                        break;
                    case "EmployeeInfo":
                        var command1 = new EmployeeInfoCommand(cmdAtr, context);
                        command1.Execute();
                        break;
                    case "EmployeePersonalInfo":
                        var command2 = new EmployeePersonalInfoCommand(cmdAtr, context);
                        command2.Execute();
                        break;
                    case "Exit":
                        var command3 = new ExitCommand(cmdAtr);
                        command3.Execute();
                        break;
                    case "ListEmployeesOlderThan":
                        var command4 = new ListEmployeesOlderThanCommand(cmdAtr, context);
                        command4.Execute();
                        break;
                    case "ManagerInfo":
                        var command5 = new ManagerInfoCommand(cmdAtr, context);
                        command5.Execute();
                        break;
                    case "SetAdress":
                        var command6 = new SetAdressCommand(cmdAtr, context);
                        command6.Execute();
                        break;
                    case "SetBirthday":
                        var command7 = new SetBirthdayCommand(cmdAtr, context);
                        command7.Execute();
                        break;
                    case "SetManager":
                        var command8 = new SetManagerCommand(cmdAtr, context);
                        command8.Execute();
                        break;
                    
                }
                 

            }
        }
    }
}
