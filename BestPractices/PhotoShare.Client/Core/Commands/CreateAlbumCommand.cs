namespace PhotoShare.Client.Core.Commands
{
    using System;
    using System.Linq;
    using Contracts;
    using PhotoShare.Client.Core.Dtos;
    using PhotoShare.Client.Utilities;
    using PhotoShare.Models.Enums;
    using Services.Contracts;


    public class CreateAlbumCommand : ICommand
    {
        private readonly IAlbumService albumService;
        private readonly IUserService userService;
        private readonly ITagService tagService;

        public CreateAlbumCommand(IAlbumService albumService, IUserService userService, ITagService tagService)
        {
            this.albumService = albumService;
            this.userService = userService;
            this.tagService = tagService;
        }

        // CreateAlbum <username> <albumTitle> <BgColor> <tag1> <tag2>...<tagN>
        public string Execute(string[] data)
        {
            var username = data[0];
            var albumTitle = data[1];
            var bgColor = data[2];
            var tags = data.Skip(3).ToArray();

            var userExists = this.userService.Exists(username);

            if (!userExists)
            {
                throw new ArgumentException($"User {username} not found!");
            }


            var albumExists = this.albumService.Exists(albumTitle);

            if (albumExists)
            {
                throw new ArgumentException($"Album {albumTitle} exists!");
            }

            var isValidColor = Enum.TryParse(bgColor, out Color result);

            if (!isValidColor)
            {
                throw new ArgumentException($"Color {bgColor} not found");
            }

            for (int i = 0; i < tags.Length; i++)
            {
                tags[1] = tags[1].ValidateOrTransform();

                var currentTag = this.tagService.Exists(tags[1]);

                if (!currentTag)
                {
                    throw new ArgumentException($"Invalid Tags!");
                }
               
            }
            var userId = this.userService.ByUsername<UserDto>(username).Id;
            this.albumService.Create(userId, albumTitle, bgColor, tags);

            return $"Album {albumTitle} was created succesfully!";
        }
    }
}
