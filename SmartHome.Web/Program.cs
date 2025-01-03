using SmartHome.Shared.Interfaces;
using SmartHome.Web.Components;
using SmartHome.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add device-specific services used by the MauiApp1.Shared project
builder.Services.AddSingleton<IFormFactor, FormFactor>();
builder.Services.AddHttpClient("AuthClient", client =>
{
    client.BaseAddress = new Uri("https://localhost:7019/api");
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
