using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using Serilog;
using Serilog.Events;
using SmartHome.API.Models;
using SmartHome.Application.Delegates;
using SmartHome.Application.Enums;
using SmartHome.Application.Health_Checks;
using SmartHome.Application.Interfaces.Health;
using SmartHome.Application.Middleware;
using SmartHome.Application.Services;
using SmartHome.Domain.Contexts;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

// add serilog

var logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .MinimumLevel.Override("Default", LogEventLevel.Fatal)
    .MinimumLevel.Override("Microsoft", LogEventLevel.Fatal)
    .MinimumLevel.Override("System", LogEventLevel.Fatal)
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

builder.Services.Configure<MongoDBConfig>(builder.Configuration.GetSection("MongoDBConfig"));

builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoDBConfig>>().Value;
    return new ApplicationDBContext(settings.ConnectionURI, settings.DatabaseName);
});

builder.Services.AddScoped<IHealthCheck, SystemHealthCheck>();

builder.Services.AddTransient<MongodbHealthCheck>();
//builder.Services.AddScoped<IMongoDBHealthRepository, MongoDBHealthRepository>();

builder.Services.AddTransient<ServiceResolver<IComponentHealthCheck>>(sp => key =>
{
    return key switch
    {
        ComponentHealthChecks.MongodbHealthCheck => sp.GetService<MongodbHealthCheck>()!,
        _ => throw new KeyNotFoundException($"Service with key {key} not found."),
    };
});

builder.Services.AddControllers();
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

app.UseAuthorization();

app.MapControllers();

app.Run();
