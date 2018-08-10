using Empoyees.App.Commands.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Empoyees.App.Commands
{
   public abstract class Command : ICommand
    {
        protected const string InvalidCommandArgumentsExceptionMessage = "Invalid command arguments!";
        protected Command(string[] cmdArgs)
        {
            CmdArgs = cmdArgs;
        }

        public string[] CmdArgs { get; set; }

        public abstract void Execute();
    }
}
