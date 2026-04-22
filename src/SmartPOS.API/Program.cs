using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmartPOS.Core.Interfaces;
using SmartPOS.Infrastructure.Data;
using SmartPOS.Infrastructure.Repositories;
using SmartPOS.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ─── Database ─────────────────────────────────────────
builder.Services.AddDbContext<SmartPOSDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("SmartPOSDB")));

// ─── Repositories & Services ──────────────────────────
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();

// ─── JWT Authentication ────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"]!;

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();

// ─── CORS (for React dashboard) ───────────────────────
builder.Services.AddCors(options =>
{
    options.AddPolicy("SmartPOSPolicy", policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("SmartPOSPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();