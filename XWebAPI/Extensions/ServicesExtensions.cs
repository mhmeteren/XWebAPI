﻿using Asp.Versioning;
using AspNetCoreRateLimit;
using Entities.Models;
using Entities.UtilityClasses.Minio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Minio;
using Minio.AspNetCore.HealthChecks;
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



        public static void ConfigureFileMangers(this IServiceCollection services, IConfiguration configuration)
        {
            var minioConfig = configuration.GetSection("MinioSettings");
            services.AddMinio(configureClient => configureClient
                .WithEndpoint(minioConfig["endpoint"])
                .WithCredentials(minioConfig["accessKey"], minioConfig["secretKey"])
                .WithSSL(bool.Parse(minioConfig["SSL"] ?? "False"))
                .Build());

            services.AddScoped<IFileUploadService, FileUploadManager>();
            services.AddSingleton<IFileDownloadService, FileDownloadManager>();
            services.Configure<CustomMinioConfig>(minioConfig);
        }



        public static void ConfigureVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(opt =>
            {
                opt.ReportApiVersions = true;
                opt.AssumeDefaultVersionWhenUnspecified = true;
                opt.DefaultApiVersion = new ApiVersion(1, 0);

                opt.ApiVersionReader = ApiVersionReader.Combine(
                    new QueryStringApiVersionReader("api-version"),
                    new HeaderApiVersionReader("api-version"),
                    new UrlSegmentApiVersionReader());
            }).AddApiExplorer(opt =>
            {
                opt.GroupNameFormat = "'v'V";
                opt.SubstituteApiVersionInUrl = true;
            });

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



        public static void ConfigureRateLimitingOptions(this IServiceCollection services,
            IConfiguration configuration)
        {

            services.Configure<IpRateLimitOptions>(configuration.GetSection("IpRateLimiting"));
            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
            services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
            services.AddSingleton<IProcessingStrategy, AsyncKeyLockProcessingStrategy>();


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
    
    
        public static void ConfigureHealthCheck(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
            .AddSqlServer(
                    connectionString: configuration.GetConnectionString("sqlConnetion"),
                    healthQuery: "SELECT 1",
                    name: "SQL Server Check",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: ["sql", "sql-server"])
            .AddRedis(
                    redisConnectionString: configuration.GetConnectionString("redisConnection"),
                    name: "Redis Check",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: ["cache", "redis"])
             .AddMinio(
                    factory: x => (MinioClient)x.GetRequiredService<IMinioClient>(),
                    name: "Minio Check",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: ["file", "minio"])
             .AddElasticsearch(
                    elasticsearchUri: configuration.GetConnectionString("elasticsearchUri"),
                    name: "Elasticsearch Check",
                    failureStatus: HealthStatus.Unhealthy,
                    tags: ["log", "elasticsearch"]
                );

           
        }

    
    }
}
