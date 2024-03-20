using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using Services.Contracts;
using XWebAPI.Extensions;




Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Async(writeTo => writeTo.Console())
    .CreateLogger();

Log.Information("Starting api host");



var builder = WebApplication.CreateBuilder(args);

//Log
builder.Host.UseSerilog((context, config) =>
{
    config.Enrich.FromLogContext()
        .ReadFrom.Configuration(context.Configuration);
});


// Add services to the container.

builder.Services.AddControllers(config =>
{
    config.RespectBrowserAcceptHeader = true;
    config.ReturnHttpNotAcceptable = true;
})
.AddApplicationPart(typeof(Presentation.AssemblyReference).Assembly)
.AddNewtonsoftJson();


builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true; //custom status code for Dto 
});


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddValidatorsFromAssemblyContaining<Presentation.AssemblyReference>();

//Extensions
builder.Services.ConfigureSqlContext(builder.Configuration);
builder.Services.ConfigureRepositoryManager();
builder.Services.ConfigureServiceManager();
builder.Services.ConfigureLoggerService();
builder.Services.ConfigureHelperServices();
builder.Services.ConfigureFileMangers(builder.Configuration);
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.ConfigureRedis(builder.Configuration);
builder.Services.ConfigureVersioning();

builder.Services.ConfigureIdentity();
builder.Services.ConfigureJWT(builder.Configuration);


var app = builder.Build();

//ExceptionMiddlewareExtensions
var logger = app.Services.GetRequiredService<ILoggerService>();
app.ConfigureExceptionHandler(logger);



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
