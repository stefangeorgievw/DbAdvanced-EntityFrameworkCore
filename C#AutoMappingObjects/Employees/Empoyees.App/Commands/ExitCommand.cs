using System;
using System.Collections.Generic;
using System.Text;

namespace Empoyees.App.Commands
{
    class ExitCommand : Command
    {
        public ExitCommand(string[] cmdArgs) : base(cmdArgs)
        {
        }

        public override void Execute()
        {
            Environment.Exit(0);
        }
    }
}
