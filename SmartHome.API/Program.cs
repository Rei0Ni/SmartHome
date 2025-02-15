using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using Serilog;
using Serilog.Events;
using SmartHome.API.Models;
using SmartHome.Application.Configuration;
using SmartHome.Application.Delegates;
using SmartHome.Enum;
using SmartHome.Application.Health_Checks;
using SmartHome.Application.Interfaces;
using SmartHome.Application.Interfaces.Health;
using SmartHome.Application.Middleware;
using SmartHome.Application.Services;
using SmartHome.Domain.Contexts;
using SmartHome.Domain.Entities;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel;
using Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption;
using Microsoft.AspNetCore.DataProtection;
using AspNetCore.Identity.Mongo;
using SmartHome.Application.Interfaces.Jwt;
using FluentValidation;
using System.Reflection.Metadata;
using System.Reflection;
using SmartHome.Application.Interfaces.User;
using SmartHome.Application.Services.User;
using SmartHome.Application.Validations;
using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.Json;
using SmartHome.Application.Interfaces.Area;
using SmartHome.Infrastructure.Repositories;
using SmartHome.Application.Interfaces.Controller;
using SmartHome.Application.Interfaces.DeviceType;
using SmartHome.Application.Interfaces.DeviceFunction;
using SmartHome.Application.Interfaces.Device;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

// add serilog

var logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Default", LogEventLevel.Information)
    //.MinimumLevel.Override("Microsoft", LogEventLevel.Fatal)
    //.MinimumLevel.Override("System", LogEventLevel.Fatal)
    .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information).WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"logs\\Info\\Info-{DateTime.Now:dd-MM-yyyy}.log"), LogEventLevel.Information, "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"))
    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug).WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"logs\\Debug\\Debug-{DateTime.Now:dd-MM-yyyy}.log"), LogEventLevel.Debug, "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"))
    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Warning).WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"logs\\Warning\\Warning-{DateTime.Now:dd-MM-yyyy}.log"), LogEventLevel.Warning, "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"))
    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error).WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"logs\\Error\\Error-{DateTime.Now:dd-MM-yyyy}.log"), LogEventLevel.Error, "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"))
    .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Fatal).WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"logs\\Fatal\\Fatal-{DateTime.Now:dd-MM-yyyy}.log"), LogEventLevel.Fatal, "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"))
    .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"logs\\Verbose\\log-{DateTime.Now:dd-MM-yyyy}.log"))
    .CreateLogger();

builder.Logging.ClearProviders();
//builder.Logging.AddSerilog(logger);
builder.Host.UseSerilog(logger);

Log.Logger = logger;

// Add services to the container.

builder.Services.AddDataProtection().UseCryptographicAlgorithms(
new AuthenticatedEncryptorConfiguration
{
    EncryptionAlgorithm = EncryptionAlgorithm.AES_256_CBC,
    ValidationAlgorithm = ValidationAlgorithm.HMACSHA256
});

builder.Services.Configure<MongoDBConfig>(builder.Configuration.GetSection("MongoDBConfig"));

builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBConfig>>().Value;
    return new ApplicationDBContext(settings.ConnectionURI, settings.DatabaseName);
});

builder.Services.AddApplicationCore();

builder.Services.AddIdentityCore<ApplicationUser>(config =>
{
    // TODO: chnge Settings For Production
    config.Password.RequiredLength = 8;
    config.Password.RequireDigit = true;
    config.Password.RequireNonAlphanumeric = true;
    config.Password.RequireUppercase = true;

    config.User.RequireUniqueEmail = true;
    config.SignIn.RequireConfirmedAccount = false;

    config.Lockout.AllowedForNewUsers = false;
    config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
    config.Lockout.MaxFailedAccessAttempts = 5;
})
.AddRoles<ApplicationRole>()
.AddMongoDbStores<ApplicationUser, ApplicationRole, Guid>
(
    mongo =>
    {
        mongo.ConnectionString = config["MongoDBConfig:ConnectionURI"] + config["MongoDBConfig:DatabaseName"];
    }
)
.AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = false,
        ValidIssuer = config["Jwt:Issuer"],
        ValidAudience = config["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!))
    };
});

// Configure global JSON options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    //options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // Ignore null values
    options.SerializerOptions.WriteIndented = true;
});

// registering repositories
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IControllerRepository, ControllerRepository>();
builder.Services.AddScoped<IDeviceTypeRepository, DeviceTypeRepository>();
builder.Services.AddScoped<IDeviceFunctionRepository, DeviceFunctionRepository>();
builder.Services.AddScoped<IDeviceRepository, DeviceRepository>();

// registering services
builder.Services.AddScoped<IHealthCheck, SystemHealthCheck>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAreaService, AreaService>();
builder.Services.AddScoped<IControllerService, ControllerService>();
builder.Services.AddScoped<IDeviceTypeService, DeviceTypeService>();
builder.Services.AddScoped<IDeviceFunctionService, DeviceFunctionService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<ICommandService, CommandService>();

builder.Services.AddHttpClient("ControllerClient");

builder.Services.AddApplicationCore();

builder.Services.AddTransient<MongodbHealthCheck>();
builder.Services.AddTransient<JwtTokenServiceHealthCheck>();
//builder.Services.AddScoped<IMongoDBHealthRepository, MongoDBHealthRepository>();

builder.Services.AddTransient<ServiceResolver<IComponentHealthCheck>>(sp => key =>
{
    return key switch
    {
        ComponentHealthChecks.MongodbHealthCheck => sp.GetService<MongodbHealthCheck>()!,
        ComponentHealthChecks.JwtTokenServiceHealthCheck => sp.GetService<JwtTokenServiceHealthCheck>()!,
        _ => throw new KeyNotFoundException($"Service with key {key} not found."),
    };
});

builder.Services.AddControllers();

// Configure global JSON options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    //options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // Ignore null values
    options.SerializerOptions.WriteIndented = true;
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandler>();

app.UseCors(options => options
             .AllowAnyMethod()
             .AllowAnyHeader()
             .SetIsOriginAllowed(origin => true)
             .AllowCredentials());

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var seedDataService = scope.ServiceProvider.GetRequiredService<ISeedDataService>();
    await seedDataService.InitializeAsync();
}

app.Run();
