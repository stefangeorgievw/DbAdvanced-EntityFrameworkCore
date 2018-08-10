using System;
using System.Collections.Generic;
using System.Text;
using TeamBuilder.App.Utilities;

namespace TeamBuilder.App.Core.Commands
{
    public class ExitCommand
    {
        public string Execute(string[] commandArgs)
        {
            Check.CheckLength(0, commandArgs);

            Environment.Exit(0);

            return "Good bye!";
        }
    }
}
