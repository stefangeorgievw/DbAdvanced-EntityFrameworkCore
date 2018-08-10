using System;
using System.Collections.Generic;
using System.Text;
using TeamBuilder.App.Utilities;

namespace TeamBuilder.App.Core.Commands
{
    public class LogoutCommand
    {
        public string Execute(string[] commandArgs)
        {
            Check.CheckLength(0, commandArgs);

            string username = AuthenticationManager.GetCurrentUser()?.Username;

            AuthenticationManager.Logout();

            return $"User {username} successfully logged out!";
        }
    }
}
