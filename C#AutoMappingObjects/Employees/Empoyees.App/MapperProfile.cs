using AutoMapper;
using Employees.Models;
using Empoyees.App.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Empoyees.App
{
    class MapperProfile:Profile
    {
        public MapperProfile()
        {
            CreateMap<EmployeeDto, Employee>();
           
        }
    }
}
