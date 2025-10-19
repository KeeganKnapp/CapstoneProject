using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using CapstoneAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// registering CapstoneDbContext with Postgres
builder.Services.AddDbContext<CapstoneDbContext>(opt => opt.UseNpgsql(builder.Configuration.GetConnectionString("Postgres")));

// add controllers for routing
builder.Services.AddControllers();

var app = builder.Build();

// map controller endpoints E.g. /api/timeentries
app.MapControllers();

app.Run();