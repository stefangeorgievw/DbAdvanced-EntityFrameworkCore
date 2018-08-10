using AutoMapper;
using ProductShop.App.DTOs;
using ProductShop.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.App
{
    class ProductShopProfile:Profile
    {
        public ProductShopProfile()
        {
            CreateMap<UserDto, User>();
            CreateMap<ProductDto, Product>();
            CreateMap<CategoryDto, Category>();
        }
    }
}
