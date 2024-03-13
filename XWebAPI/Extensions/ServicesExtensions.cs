using Entities.Models;
using Entities.UtilityClasses.Minio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Presentation.ActionFilters;
using Repositories.Contracts;
using Repositories.EFCore;
using Services;
using Services.Contracts;
using StackExchange.Redis;
using System.Text;

namespace XWebAPI.Extensions
{
    public static class ServicesExtensions
    {

        public static void ConfigureSqlContext(this IServiceCollection services,IConfiguration configuration) => 
            services.AddDbContext<RepositoryContext>(options => 
                options.UseSqlServer(configuration.GetConnectionString("sqlConnetion")));


        public static void ConfigureRepositoryManager(this IServiceCollection services) =>
             services.AddScoped<IRepositoryManager, RepositoryManager>();

        public static void ConfigureServiceManager(this IServiceCollection services) =>
            services.AddScoped<IServiceManager, ServiceManager>();

        public static void ConfigureLoggerService(this IServiceCollection services) =>
            services.AddSingleton<ILoggerService, SerilogLoggerManager>();


        public static void ConfigureHelperServices(this IServiceCollection services)
        {
            services.AddSingleton<IValidatorService, ValidatorManager>();
        }

        public static void ConfigureActionFilters(this IServiceCollection services)
        {
            services.AddScoped<ValidationFilterAttribute>();
        }


        public static void ConfigureFileMangers(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IFileUploadService, FileUploadManager>();
            services.AddSingleton<IFileDownloadService, FileDownloadManager>();
            services.Configure<CustomMinioConfig>(configuration.GetSection("MinioSettings"));
        }

        public static void ConfigureRedis(this IServiceCollection services,
        IConfiguration configuration)
        {

            var redisConnectionString = configuration.GetConnectionString("redisConnection");

            var retryCount = 3;
            var retryDelayMilliseconds = 100;

            var multiplexer = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                EndPoints = { redisConnectionString },
                ConnectRetry = retryCount,
                ConnectTimeout = retryDelayMilliseconds * retryCount,
                SyncTimeout = retryDelayMilliseconds * retryCount
            });

            services.AddSingleton<IConnectionMultiplexer>(multiplexer);
            services.AddSingleton<ICacheService, RedisCacheManager>();
        }


        public static void ConfigureIdentity(this IServiceCollection services)
        {
            var builder = services.AddIdentity<BaseUser, IdentityRole>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
                opts.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._";
                opts.Password.RequireDigit = true;
                opts.Password.RequireLowercase = true;
                opts.Password.RequireUppercase = true;
                opts.Password.RequireNonAlphanumeric = true;
                opts.Password.RequiredLength = 8;
                opts.Password.RequiredUniqueChars = 5;
                opts.SignIn.RequireConfirmedEmail = true;

            })
             .AddEntityFrameworkStores<RepositoryContext>()
             .AddDefaultTokenProviders();
        }

        public static void ConfigureJWT(this IServiceCollection services, IConfiguration config)
        {
            var jwtSettings = config.GetSection("JwtSettings");
            var secretKey = jwtSettings["secretKey"];

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            }).AddJwtBearer(opts =>
            opts.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["validIssuer"],
                ValidAudience = jwtSettings["validAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))

            });

        }
    }
}
