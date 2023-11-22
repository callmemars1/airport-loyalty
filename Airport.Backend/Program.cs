using Airport.Backend.Endpoints;
using Airport.Backend.Utils;
using Airport.Model;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var services = builder.Services;

services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/auth/sign-in";
        options.LogoutPath = "/api/auth/sign-out";
    });

services.AddAuthorization();

services.AddDbContext<AirportDbContext>(
    options => options
        .UseNpgsql(config.GetConnectionString("AirportContextConnectionString"))
        .UseLazyLoadingProxies()
        .UseSnakeCaseNamingConvention());

services.AddTransient<IPasswordHasher, HMACSHA256PasswordHasher>();

services.AddScoped<IValidator<SignUpRequest>, SignUpRequestValidator>();

services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

var apiEndpoints = app.MapGroup("/api");
apiEndpoints.RegisterAuthEndpoints();

app.Run();