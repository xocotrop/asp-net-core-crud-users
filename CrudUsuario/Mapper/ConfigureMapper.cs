using AutoMapper;
using CrudUsuario.Entity;
using CrudUsuario.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CrudUsuario.Mapper
{
    public class ConfigureMapper : Profile
    {
        public ConfigureMapper()
        {
            CreateMap<User, UserResponse>();
        }
    }
}
