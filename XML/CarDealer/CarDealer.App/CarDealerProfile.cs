using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using CarDealer.App.Dtos.Import;
using CarDealer.Models;

namespace CarDealer.App
{
    class CarDealerProfile:Profile
    {
        public CarDealerProfile()
        {
            CreateMap<SupplierDto, Supplier>();
            CreateMap<PartDto, Part>();
            CreateMap<CarDto, Car>();
            CreateMap<CustomertDto, Customer>();
        }
       
           
    }
}
