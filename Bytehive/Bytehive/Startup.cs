using AutoMapper;
using Bytehive.Controllers;
using Bytehive.Data;
using Bytehive.Models.Settings;
using Bytehive.Notifications;
using Bytehive.Scraper;
using Bytehive.Scraper.Contracts;
using Bytehive.Services;
using Bytehive.Services.Authentication;
using Bytehive.Services.Contracts.Repository;
using Bytehive.Services.Contracts.Services;
using Bytehive.Services.Repository;
using Bytehive.Services.Utilities;
using Bytehive.Storage;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Security.Claims;
using System.Text;

namespace Bytehive
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services
                .AddDbContext<BytehiveDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("BytehiveConnection")));

            this.RegisterCoreDIDependencies(services);

            services.AddAutoMapper(typeof(IUsersRepository).Assembly, typeof(AccountController).Assembly);

            var authSettings = Configuration.GetSection(nameof(AuthSettings));
            services.Configure<AuthSettings>(authSettings);

            var signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(authSettings[nameof(AuthSettings.SecretKey)]));

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            services.Configure<JwtIssuerOptions>(options =>
            {
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            });

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                RequireExpirationTime = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            services.AddHangfire(configuration => configuration
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(Configuration.GetConnectionString("BytehiveHangfireConnection"), new SqlServerStorageOptions
                {
                    CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                    SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                    QueuePollInterval = TimeSpan.Zero,
                    UseRecommendedIsolationLevel = true,
                    UsePageLocksOnDequeue = true,
                    DisableGlobalLocks = true
                }));

            services.AddHangfireServer();

            services.AddCors();
            services.AddControllers();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(configureOptions =>
            {
                configureOptions.ClaimsIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                configureOptions.TokenValidationParameters = tokenValidationParameters;
                configureOptions.SaveToken = true;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(Constants.Strings.Roles.User, policy => policy.RequireClaim(ClaimTypes.Role, Constants.Strings.JwtClaims.ApiUser, Constants.Strings.JwtClaims.ApiAdministrator));
                options.AddPolicy(Constants.Strings.Roles.Administrator, policy => policy.RequireClaim(ClaimTypes.Role, Constants.Strings.JwtClaims.ApiAdministrator));
            });
        }

        public void Configure(IApplicationBuilder app, IBackgroundJobClient backgroundJobs, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            var scraperProcessor = serviceProvider.GetService<IScraperProcessor>();
            RecurringJob.AddOrUpdate("request-processor", () => scraperProcessor.ProcessScrapeRequest(), "* * * * *");

            app.UseCors(
                options => options.AllowAnyHeader().AllowAnyMethod().WithOrigins("http://localhost:4200", "https://www.bytehive.com")
            );

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void RegisterCoreDIDependencies(IServiceCollection services)
        {
            services.AddTransient<IUsersRepository, UsersRepository>();
            services.AddTransient<IRolesRepository, RolesRepository>();
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient<IScrapeRequestsRepository, ScrapeRequestsRepository>();
            services.AddTransient<IUserRolesRepository, UserRolesRepository>();
            services.AddTransient<IFilesRepository, FilesRepository>();

            services.AddTransient<IAccountService, AccountService>();
            services.AddTransient<IUsersService, UsersService>();
            services.AddTransient<IScrapeRequestsService, ScrapeRequestsService>();
            services.AddTransient<IFilesService, FilesService>();

            services.AddTransient<ISendGridSender, SendGridSender>();
            services.AddTransient<IScraperClient, ScraperClient>();
            services.AddTransient<IScraperParser, ScraperParser>();
            services.AddTransient<IScraperFileHelper, ScraperFileHelper>();
            services.AddTransient<IScraperProcessor, ScraperProcessor>();

            services.AddTransient<IAzureBlobStorageProvider, AzureBlobStorageProvider>();

        }
    }
}
