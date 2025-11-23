using CapstoneBlazorApp.Components;
using CapstoneBlazorApp.Services;
using CapstoneBlazorApp.Services.Abstractions;
using CapstoneBlazorApp.Services.Auth;
using MudBlazor.Services;
using MudBlazor;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


builder.Services.AddHttpClient("CapstoneAPI", client =>
{
    client.BaseAddress = new Uri("http://localhost:5162/");
});


//add mudblazor services
builder.Services.AddMudServices();


builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("CapstoneAPI"));

builder.Services.AddScoped<IAuthService, AuthService>();
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
