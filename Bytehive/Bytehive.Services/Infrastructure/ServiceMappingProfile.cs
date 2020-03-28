using AutoMapper;
using Bytehive.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Services.Infrastructure
{
    public class ServiceMappingProfile : Profile
    {
        public ServiceMappingProfile()
        {
            this.CreateMap<User, User>();
            this.CreateMap<Role, Role>();
            this.CreateMap<RefreshToken, RefreshToken>();
            this.CreateMap<UserRole, UserRole>();
        }
    }
}
