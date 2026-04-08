using FluentValidation;
using FluentValidation.AspNetCore;
using DemoSocialMedia.Api;
using DemoSocialMedia.Application.Auth.Services;
using DemoSocialMedia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.Load("DemoSocialMedia.Application")));

builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();
builder.Services.AddScoped<IEmailSender, EmailSenderMock>();
builder.Services.AddScoped<IVerificationTokenGenerator, VerificationTokenGenerator>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<DemoSocialMedia.Application.Auth.Validators.RegisterUserRequestValidator>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.OperationFilter<DemoSocialMedia.Api.Swagger.FileUploadOperationFilter>();
});

var defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var healthChecks = builder.Services.AddHealthChecks();
if (!string.IsNullOrWhiteSpace(defaultConnectionString))
{
    healthChecks.AddNpgSql(defaultConnectionString, name: "PostgreSQL");
}

builder.Services.AddScoped<DemoSocialMedia.Infrastructure.Services.IMinioService, DemoSocialMedia.Infrastructure.Services.MinioService>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
                  "http://localhost:3000",
                  "http://localhost:9000",
                  "https://localhost:3000"
               )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddSignalR();

// JWT Authentication (temel yapı, test için AllowAnonymous kullanılabilir)
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException("JWT key configuration missing. Please set 'Jwt:Key' in configuration (e.g. appsettings.Development.json).");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

var app = builder.Build();

if (!string.Equals(Environment.GetEnvironmentVariable("SKIP_DATABASE_MIGRATION"), "true", StringComparison.OrdinalIgnoreCase))
{
    await using var scope = app.Services.CreateAsyncScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseMiddleware<DemoSocialMedia.Api.Middleware.JwtCookieToHeaderMiddleware>();
app.UseAuthentication();
app.UseMiddleware<DemoSocialMedia.Api.Middleware.UserIdMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.MapHealthChecks("/health");
app.Run();

public partial class Program { }