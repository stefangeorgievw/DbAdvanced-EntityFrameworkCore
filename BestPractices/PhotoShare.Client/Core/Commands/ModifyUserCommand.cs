namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Linq;
    using Contracts;
    using PhotoShare.Client.Core.Dtos;
    using PhotoShare.Services.Contracts;

    public class ModifyUserCommand : ICommand
    {
        private readonly IUserService userService;
        private readonly ITownService townService;
        public ModifyUserCommand(IUserService userService, ITownService townService)
        {
            this.userService = userService;
            this.townService = townService;
        }

        // ModifyUser <username> <property> <new value>
        // For example:
        // ModifyUser <username> Password <NewPassword>
        // ModifyUser <username> BornTown <newBornTownName>
        // ModifyUser <username> CurrentTown <newCurrentTownName>
        // !!! Cannot change username
        public string Execute(string[] data)
        {
            var username = data[0];
            var property = data[1];
            var value = data[2];

            var userExists = this.userService.Exists(username);
            if (!userExists)
            {
                throw new ArgumentException($"User {username} was not found!");
            }

            var userId = this.userService.ByUsername<UserDto>(username).Id;

            if (property == "Password")
            {
                SetPassword(userId, value);
            }
            else if (property == "BornTown")
            {
                SetBornTown(userId, value);
            }
            else if (property == "CurrentTown")
            {
                SetCurrentTown(userId, value);
            }
            else
            {
                throw new ArgumentException($"Propery {property} not supported!");
            }

            return $"User {username} {property} is {value}.";

        }

        private void SetCurrentTown(int userId, string name)
        {

            var townExists = this.townService.Exists(name);

            if (!townExists)
            {
                throw new ArgumentException($"Value {name} not valid.\nTown {name} not found!");
            }

            var townId = this.townService.ByName<TownDto>(name).Id;
            this.userService.SetCurrentTown(userId, townId);
        }

        private void SetBornTown(int userId, string name)
        {
            var townExists = this.townService.Exists(name);

            if (!townExists)
            {
                throw new ArgumentException($"Value {name} not valid.\nTown {name} not found!");
            }

            var townId = this.townService.ByName<TownDto>(name).Id;
            this.userService.SetBornTown(userId, townId);
        }

        private void SetPassword(int userId, string value)
        {
            var isLower = value.Any(x => char.IsLower(x));
            var isDigit = value.Any(x => char.IsDigit(x));
            if (!isDigit || !isLower)
            {
                throw new ArgumentException($"Value {value} not valid.\nInvalid Password");
            }

            this.userService.ChangePassword(userId, value);
        }
    }
}
