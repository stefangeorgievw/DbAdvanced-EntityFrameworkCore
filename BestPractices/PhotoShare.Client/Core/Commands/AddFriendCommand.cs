namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Linq;
    using Contracts;
    using PhotoShare.Client.Core.Dtos;
    using PhotoShare.Services.Contracts;

    public class AddFriendCommand : ICommand
    {
        private readonly IUserService userService;
        public AddFriendCommand(IUserService userService)
        {
            this.userService = userService;
        }

        // AddFriend <username1> <username2>
        public string Execute(string[] data)
        {
            string username = data[0];
            string friendUsername = data[1];

            var userExists = this.userService.Exists(username);
            var friendExists = this.userService.Exists(friendUsername);

            if (!userExists)
            {
                throw new ArgumentException($"User {username} not found!");
            }

            if (!friendExists)
            {
                throw new ArgumentException($"User {friendUsername} not found!");
            }

            var user = this.userService.ByUsername<UserFriendsDto>(username);
            var friend = this.userService.ByUsername<UserFriendsDto>(friendUsername);

            bool isSendRequestFromUser = user.Friends.Any(x => x.Username == friend.Username);
                 bool isSendRequrstFromFriend=  friend.Friends.Any(x => x.Username == user.Username);

            if (isSendRequestFromUser && isSendRequrstFromFriend)
            {
                throw new InvalidOperationException($"{friend.Username} is already friend to {user.Username}");
            }

           else  if (isSendRequestFromUser && !isSendRequrstFromFriend)
            {
                throw new InvalidOperationException("Reques is already send");
            }

           else if (!isSendRequestFromUser && isSendRequrstFromFriend)
            {
                throw new InvalidOperationException("Reques is already send");
            }


            this.userService.AddFriend(user.Id, friend.Id);

            return $"Friend {friend.Username} added to {user.Username}";

        }
    }
}
