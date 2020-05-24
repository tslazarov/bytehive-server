using AutoMapper;
using Bytehive.Data.Models;
using Bytehive.Models.Account;
using Bytehive.Models.Payment;
using Bytehive.Models.ScrapeRequests;
using Bytehive.Models.Statistics;
using Bytehive.Models.Users;
using Bytehive.Services.Utilities;
using Newtonsoft.Json;
using PayPalCheckoutSdk.Orders;
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
                .ForMember(m => m.RegistrationDate, map => map.MapFrom(source => DateTime.UtcNow));

            // User
            this.CreateMap<User, UserListViewModel>()
                .ForMember(m => m.TotalRequests, map => map.MapFrom(source => source.ScrapeRequests.Count));

            this.CreateMap<User, UserDetailViewModel>()
                .ForMember(m => m.ScrapeRequests, map => map.MapFrom(source => source.ScrapeRequests))
                .ForMember(m => m.Payments, map => map.MapFrom(source => source.Payments));

            // ScrapeRequest
            this.CreateMap<ScrapeRequestCreateModel, ScrapeRequest>()
                .ForMember(m => m.Id, map => map.MapFrom(source => Guid.NewGuid()))
                .ForMember(m => m.Data, map => map.MapFrom(source => JsonConvert.SerializeObject(source)))
                .ForMember(m => m.Status, map => map.MapFrom(source => ScrapeRequestStatus.Pending))
                .ForMember(m => m.CreationDate, map => map.MapFrom(source => DateTime.UtcNow));

            this.CreateMap<ScrapeRequest, ScrapeRequestListViewModel>()
                .ForMember(m => m.Email, map => map.MapFrom(source => source.User != null ? source.User.Email : string.Empty))
                .ForMember(m => m.FileName, map => map.MapFrom(source => source.File != null ? source.File.Name : string.Empty))
                .ForMember(m => m.ContentType, map => map.MapFrom(source => source.File != null ? source.File.ContentType : string.Empty));

            this.CreateMap<ScrapeRequest, ScrapeRequestProfileListViewModel>()
                .ForMember(m => m.FileName, map => map.MapFrom(source => source.File != null ? source.File.Name : string.Empty))
                .ForMember(m => m.ContentType, map => map.MapFrom(source => source.File != null ? source.File.ContentType : string.Empty));

            this.CreateMap<ScrapeRequest, ScrapeRequestDetailViewModel>()
                .ForMember(m => m.Email, map => map.MapFrom(source => source.User != null ? source.User.Email : string.Empty));

            this.CreateMap<PaymentTier, PaymentTierListViewModel>();

            this.CreateMap<Payment, PaymentListViewModel>()
                .ForMember(m => m.Email, map => map.MapFrom(source => source.User != null ? source.User.Email : string.Empty));

            this.CreateMap<Payment, PaymentProfileListViewModel>()
                .ForMember(m => m.TierName, map => map.MapFrom(source => source.PaymentTier != null ? source.PaymentTier.Name : string.Empty))
                .ForMember(m => m.TierValue, map => map.MapFrom(source => source.PaymentTier != null ? source.PaymentTier.Value : 0));

            this.CreateMap<Payment, PaymentDetailViewModel>()
                .ForMember(m => m.Email, map => map.MapFrom(source => source.User != null ? source.User.Email : string.Empty))
                .ForMember(m => m.TierName, map => map.MapFrom(source => source.PaymentTier != null ? source.PaymentTier.Name : string.Empty))
                .ForMember(m => m.TierSku, map => map.MapFrom(source => source.PaymentTier != null ? source.PaymentTier.Sku : string.Empty))
                .ForMember(m => m.TierValue, map => map.MapFrom(source => source.PaymentTier != null ? source.PaymentTier.Value : 0));

            // Statistics
            this.CreateMap<User, UsersSummaryViewModel>();
        }
    }
}
