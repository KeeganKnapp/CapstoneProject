
using System.Text;
using System.Security.Claims;
using CapstoneAPI.Data;
using CapstoneAPI.Helpers;
using CapstoneAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;       
using Microsoft.EntityFrameworkCore;                      
using Microsoft.Extensions.Options;                        
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);
var cfg = builder.Configuration;


builder.Services.AddControllers();

builder.Services.AddDbContext<CapstoneDbContext>(options => options.UseNpgsql(cfg.GetConnectionString("DefaultConnection")));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Capstone API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(p => p.AddPolicy("client", policy =>
    policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin()
));

builder.Services.Configure<JwtOptions>(cfg.GetSection("Jwt"));

var jwtSection = cfg.GetSection("Jwt");

var secret = jwtSection["Secret"] ?? jwtSection["Key"];
if (string.IsNullOrWhiteSpace(secret))
{
    throw new InvalidOperationException("Missing JWT secret. Set Jwt:Secret (preferred) or Jwt:Key in appsettings.json.");
}

var signingKey = new SymmetricSecurityKey(Convert.FromBase64String(secret));

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSection["Issuer"],

            ValidateAudience = true,
            ValidAudience = jwtSection["Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,

            ValidateLifetime = true,                 // reject expired tokens
            ClockSkew = TimeSpan.FromSeconds(15),

            RoleClaimType = ClaimTypes.Role
        };
    });

var managerEmails = builder.Configuration.GetSection("ManagerEmails").Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddSingleton(managerEmails);

builder.Services.AddAuthorization();

builder.Services.AddScoped<ITokenService, TokenService>(); // creates/rotates tokens
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IJobsiteService, JobsiteService>();
builder.Services.AddScoped<IAssignmentService, AssignmentService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("client");

app.UseAuthentication(); 
app.UseAuthorization();

app.MapControllers();
app.Run();