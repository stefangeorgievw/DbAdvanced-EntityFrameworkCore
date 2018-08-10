using System;
using TeamBuilder.App.Core;
using TeamBuilder.App.Utilities;

namespace TeamBuilder.App
{
    class Program
    {
        public static void Main()
        {
            DbTools.ResetDb();

            Engine engine = new Engine(new CommandDispatcher());
            engine.Run();
        }
    }
}
