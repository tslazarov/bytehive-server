using Autofac;
using Bytehive.Scraper;
using Bytehive.Scraper.Contracts;
using Bytehive.Services.Authentication;
using Bytehive.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bytehive
{
    public class InfrastructureModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<JwtFactory>().As<IJwtFactory>().SingleInstance();
            builder.RegisterType<JwtTokenHandler>().As<IJwtTokenHandler>().SingleInstance();
            builder.RegisterType<TokenFactory>().As<ITokenFactory>().SingleInstance();
            builder.RegisterType<JwtTokenValidator>().As<IJwtTokenValidator>().SingleInstance();
            builder.RegisterType<ExternalTokenValidator>().As<IExternalTokenValidator>().SingleInstance();
            builder.RegisterType<ScraperParser>().As<IScraperParser>().SingleInstance();

            builder.RegisterType<Services.AppConfig.AppConfiguration>().As<Services.AppConfig.IAppConfiguration>().SingleInstance();
            builder.RegisterType<Notifications.AppConfig.AppConfiguration>().As<Notifications.AppConfig.IAppConfiguration>().SingleInstance();
            builder.RegisterType<Scraper.AppConfig.AppConfiguration>().As<Scraper.AppConfig.IAppConfiguration>().SingleInstance();
            builder.RegisterType<Storage.AppConfig.AppConfiguration>().As<Storage.AppConfig.IAppConfiguration>().SingleInstance();
        }
    }
}
