using AutoMapper;
using Bytehive.Data.Models;
using Bytehive.Models.Account;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Services.Infrastructure
{
    public class ControllerMappingProfile : Profile
    {
        public ControllerMappingProfile()
        {
            this.CreateMap<SignupUserModel, User>()
                .ForMember(m => m.Id, map => map.MapFrom(source => Guid.NewGuid()));
        }
    }
}
