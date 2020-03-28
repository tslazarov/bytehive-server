using Autofac;
using Bytehive.Services.AppConfig;
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
            builder.RegisterType<AppConfiguration>().As<IAppConfiguration>().SingleInstance();
        }
    }
}
