using FluentValidation;
using FluentValidation.AspNetCore;
using DemoSocialMedia.Api;
using DemoSocialMedia.Application.Auth.Services;
using DemoSocialMedia.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
if (!string.IsNullOrWhiteSpace(defaultConnectionString))
{
    builder.Services.AddHealthChecks().AddNpgSql(defaultConnectionString, name: "PostgreSQL");
}

builder.Services.AddScoped<DemoSocialMedia.Infrastructure.Services.MinioService>();

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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("mySuperSecretKeyForJwtToken1234567890!!"))
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.Use(async (context, next) =>
{
    var token = context.Request.Cookies["token"];
    Console.WriteLine($"[MIDDLEWARE] Cookie token: {token}");
    if (!string.IsNullOrEmpty(token) && !context.Request.Headers.ContainsKey("Authorization"))
    {
        context.Request.Headers.Append("Authorization", $"Bearer {token}");
        Console.WriteLine("[MIDDLEWARE] Authorization header eklendi.");
    }
    await next();
});

app.UseMiddleware<DemoSocialMedia.Api.Middleware.JwtCookieToHeaderMiddleware>();
app.UseAuthentication();
app.UseMiddleware<DemoSocialMedia.Api.Middleware.UserIdMiddleware>();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chathub");

app.MapHealthChecks("/health");
app.Run();