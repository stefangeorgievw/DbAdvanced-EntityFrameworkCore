using System;
using System.Collections.Generic;
using System.Text;
using TeamBuilder.Data;

namespace TeamBuilder.App.Utilities
{
    internal class DbTools
    {
        internal static void ResetDb()
        {
            using (var context = new TeamBuilderContext())
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
            }
        }
    }
}
