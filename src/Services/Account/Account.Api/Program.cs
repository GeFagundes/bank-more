using Account.Api.Middleware;
using Account.Application.Services;
using Account.Domain.Exceptions;
using Account.Domain.Interfaces;
using Account.Infra.Context;
using Account.Infra.Repositories;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

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

var app = builder.Build();

// --- EXECUTION PIPELINE ---
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();