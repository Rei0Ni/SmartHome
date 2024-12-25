using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Events;
using SmartHome.API.Models;
using SmartHome.Application.Interfaces.MongoDBHealth.Repository;
using SmartHome.Application.Interfaces.MongoDBHealth.Service;
using SmartHome.Application.Middleware;
using SmartHome.Application.Services;
using SmartHome.Domain.Contexts;
using SmartHome.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddScoped<IMongoDBHealth, MongoDBHealth>();
builder.Services.AddScoped<IMongoDBHealthRepository, MongoDBHealthRepository>();

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
