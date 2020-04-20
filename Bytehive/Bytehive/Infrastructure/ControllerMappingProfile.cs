﻿using AutoMapper;
using Bytehive.Data.Models;
using Bytehive.Models.Account;
using Bytehive.Models.ScrapeRequests;
using Bytehive.Models.Users;
using Bytehive.Services.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bytehive.Services.Infrastructure
{
    public class ControllerMappingProfile : Profile
    {
        public ControllerMappingProfile()
        {
            // Account
            this.CreateMap<SignupUserModel, User>()
                .ForMember(m => m.Id, map => map.MapFrom(source => Guid.NewGuid()))
                .ForMember(m => m.Provider, map => map.MapFrom(source => Constants.Strings.UserProviders.DefaultProvider))
                .ForMember(m => m.RegistrationDate, map => map.MapFrom(source => DateTime.Now));

            // User
            this.CreateMap<User, UserListViewModel>();

            // ScrapeRequest
            this.CreateMap<ScrapeRequestCreateModel, ScrapeRequest>()
                .ForMember(m => m.Id, map => map.MapFrom(source => Guid.NewGuid()))
                .ForMember(m => m.Data, map => map.MapFrom(source => JsonConvert.SerializeObject(source)))
                .ForMember(m => m.Status, map => map.MapFrom(source => ScrapeRequestStatus.Pending))
                .ForMember(m => m.CreationDate, map => map.MapFrom(source => DateTime.Now));
        }
    }
}
