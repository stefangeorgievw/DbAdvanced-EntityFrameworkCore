using AutoMapper.QueryableExtensions;
using PhotoShare.Data;
using PhotoShare.Models;
using PhotoShare.Models.Enums;
using PhotoShare.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotoShare.Services
{
    public class AlbumService : IAlbumService
    {
        private PhotoShareContext context;

        public AlbumService(PhotoShareContext context)
        {
            this.context = context;
        }

        public TModel ById<TModel>(int id)
         => By<TModel>(a => a.Id == id).SingleOrDefault();

        public TModel ByName<TModel>(string name)
         => By<TModel>(a => a.Name == name).SingleOrDefault();

        public Album Create(int userId, string albumTitle, string BgColor, string[] tags)
        {
            var backgroundColor = Enum.Parse<Color>(BgColor, true);
            var album = new Album
            {
                Name = albumTitle,
                BackgroundColor = backgroundColor
            };

            this.context.Albums.Add(album);
            this.context.SaveChanges();

            var albumRole = new AlbumRole
            {
                UserId = userId,
                Album = album
            };

            this.context.AlbumRoles.Add(albumRole);
            this.context.SaveChanges();

            foreach (var tag in tags)
            {
                var currentTag = this.context.Tags.FirstOrDefault(t => t.Name == tag).Id;

                var albumTag = new AlbumTag
                {
                    TagId = currentTag,
                    Album = album
                };

            this.context.AlbumTags.Add(albumTag);
            }

            this.context.SaveChanges();

            return album;

        }

        public bool Exists(int id)
        => ById<Album>(id) != null;

        public bool Exists(string name)
       => ByName<Album>(name) != null;

        private IEnumerable<TModel> By<TModel>(Func<Album, bool> predicate)
         => this.context.Albums.Where(predicate).AsQueryable().ProjectTo<TModel>();

    }
}
