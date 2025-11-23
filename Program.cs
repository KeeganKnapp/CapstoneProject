using CapstoneBlazorApp.Components;
using CapstoneBlazorApp.Services;
using CapstoneBlazorApp.Services.Abstractions;
using CapstoneBlazorApp.Services.Auth;
using MudBlazor.Services;
using MudBlazor;
using Microsoft.AspNetCore.Components.Authorization;
using System.IdentityModel.Tokens.Jwt;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add cascading authentication state for Blazor components (client-side only)
builder.Services.AddCascadingAuthenticationState();

builder.Services.AddHttpClient("CapstoneAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5162/");
});

//add mudblazor services
builder.Services.AddMudServices();


builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("CapstoneAPI"));

// Authentication services
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(provider => provider.GetRequiredService<CustomAuthenticationStateProvider>());
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddScoped<JobsiteApiService>();
builder.Services.AddScoped<RequestOffApiService>();
builder.Services.AddScoped<TimeEntryApiService>();
builder.Services.AddScoped<LocationManager>();
builder.Services.AddScoped<NavigationTracker>();
builder.Services.AddScoped<AbstractLoggerService, LoggerService>();


var app = builder.Build();
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
