using System.Text.Json;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Http.Json;
using SmartHome.Shared.Interfaces;
using SmartHome.Shared.Providers;
using SmartHome.Shared.Services;
using SmartHome.Web.Components;
using SmartHome.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.SetMinimumLevel(LogLevel.Trace);
builder.Logging.AddConsole();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJwtStorageService, JwtStorageService>();
builder.Services.AddScoped<JwtAuthStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(s => s.GetRequiredService<JwtAuthStateProvider>());

// Add services used by the SmartHome.Web project
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<ISecureStorageService, SecureStorageService>();
builder.Services.AddScoped<IPlatformDetectionService, PlatformDetectionService>();
builder.Services.AddScoped<IHostConfigurationCheckService, HostConfigurationCheckService>();

builder.Services.AddBlazorBootstrap();

// Configure global JSON options
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    //options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull; // Ignore null values
    options.SerializerOptions.WriteIndented = true;
});

builder.Services.AddHttpClient("AuthClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:62061/");
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies(typeof(SmartHome.Shared._Imports).Assembly);

app.Run();
