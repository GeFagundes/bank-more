using Account.Api.Middleware;
using Account.Application.Services;
using Account.Domain.Exceptions;
using Account.Domain.Interfaces;
using Account.Infra.Context;
using Account.Infra.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- INFRASTRUCTURE CONFIGURATION ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=bankmore.db";

builder.Services.AddDbContext<AccountDbContext>(options => options.UseSqlite(connectionString));

// --- DEPENDENCY INJECTION (LAYERS) ---
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<AccountService>();

// --- GLOBAL ERROR HANDLING ---
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

// --- OPENAPI & DOCUMENTATION ---
builder.Services.AddControllers();
builder.Services.AddOpenApi();

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = Encoding.ASCII.GetBytes(jwtSettings["Secret"] ?? throw new InvalidOperationException("JWT Secret not configured. "));

// AUTHENTICATION CONFIGURATION - JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Issuer validation
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],

        // Validates the token
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],

        // Validates the digital signature of the token
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),

        ValidateLifetime = true,

        // Invalidate the addition 5 minutes period given by the .NET
        ClockSkew = TimeSpan.Zero
    };
});

// AUTHORIZATION CONFIGURATION
builder.Services.AddAuthorization();

var app = builder.Build();

// --- EXECUTION PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();