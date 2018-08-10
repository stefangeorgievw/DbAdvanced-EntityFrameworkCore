﻿using AutoMapper.QueryableExtensions;
using PhotoShare.Data;
using PhotoShare.Models;
using PhotoShare.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PhotoShare.Services
{
    public class UserService : IUserService
    {
        private readonly PhotoShareContext context;

        public UserService(PhotoShareContext context)
        {
            this.context = context;
        }
        public Friendship AcceptFriend(int userId, int friendId)
        {
            var friendship = new Friendship
            {
                UserId = userId,
                FriendId = friendId
            };

            this.context.Friendships.Add(friendship);

            this.context.SaveChanges();

            return friendship;
        }

        public Friendship AddFriend(int userId, int friendId)
        {
            var friendship = new Friendship
            {
                UserId = userId,
                FriendId = friendId
            };

            this.context.Friendships.Add(friendship);

            this.context.SaveChanges();

            return friendship;
        }

        public TModel ById<TModel>(int id)
          => By<TModel>(t => t.Id == id).SingleOrDefault();

        public TModel ByUsername<TModel>(string username)
              => By<TModel>(t => t.Username == username).SingleOrDefault();
        

        public void ChangePassword(int userId, string password)
        {
            var user = this.ById<User>(userId);

            user.Password = password;

            this.context.SaveChanges();
        }

        public void Delete(string username)
        {
            var user = this.ByUsername<User>(username);

            user.IsDeleted = true;

            this.context.SaveChanges();
        }

        public bool Exists(int id)
        => ById<User>(id) != null;

        public bool Exists(string name)
         => ByUsername<User>(name) != null;

        public User Register(string username, string password, string email)
        {
            var user = new User
            {
                Username = username,
                Password = password,
                Email = email,
                IsDeleted = false
            };

            this.context.Users.Add(user);

            this.context.SaveChanges();

            return user;
        }

        public void SetBornTown(int userId, int townId)
        {
            var user = this.ById<User>(userId);

            user.BornTownId = townId;

            this.context.SaveChanges();
        }

        public void SetCurrentTown(int userId, int townId)
        {
            var user = this.ById<User>(userId);

            user.CurrentTownId = townId;

            this.context.SaveChanges();
        }

        private IEnumerable<TModel> By<TModel>(Func<User, bool> predicate)
           => this.context.Users.Where(predicate).AsQueryable().ProjectTo<TModel>();
    }
}