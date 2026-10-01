using System.Text;
using DemoSocialMedia.Api;
using DemoSocialMedia.Api.Controllers;
using DemoSocialMedia.Application.Auth.Commands;
using DemoSocialMedia.Application.Auth.Services;
using DemoSocialMedia.Infrastructure.Persistence;
using DemoSocialMedia.Infrastructure.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey) || Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("'Jwt:Key' en az 32 bayt olmalı (HMAC-SHA256). appsettings.Development.json veya ortam değişkeni ile verin.");

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RegisterUserCommand>());

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IEmailSender, EmailSenderMock>();
builder.Services.AddScoped<IVerificationTokenGenerator, VerificationTokenGenerator>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddSingleton<IMinioService, MinioService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterUserCommand>();

builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var healthChecks = builder.Services.AddHealthChecks();
if (!string.IsNullOrWhiteSpace(defaultConnectionString))
{
    healthChecks.AddNpgSql(defaultConnectionString, name: "PostgreSQL");
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy => policy
        .WithOrigins(builder.Configuration.GetSection("Cors:Origins").Get<string[]>() ?? [])
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

builder.Services.AddSignalR();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
        // Tarayıcı istemcileri (SignalR dahil) token'ı HttpOnly cookie'de taşır; Authorization header varsa o kazanır.
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                if (!ctx.Request.Headers.ContainsKey("Authorization"))
                    ctx.Token = ctx.Request.Cookies[AuthController.TokenCookie];
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

if (!string.Equals(Environment.GetEnvironmentVariable("SKIP_DATABASE_MIGRATION"), "true", StringComparison.OrdinalIgnoreCase))
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");
app.MapHealthChecks("/health");
app.Run();

public partial class Program { }
