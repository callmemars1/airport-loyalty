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
        // Disable automatic redirection for authentication challenges (401 Unauthorized)
        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Task.CompletedTask;
        };

        // Disable automatic redirection for forbidden access (403 Forbidden)
        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return Task.CompletedTask;
        };
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
apiEndpoints.RegisterFlightsEndpoints();

app.Run();