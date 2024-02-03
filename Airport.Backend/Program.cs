using Airport.Backend.Endpoints;
using Airport.Backend.Utils;
using Airport.Model;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
var services = builder.Services;

services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(@"data-protection-keys"));

services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.LoginPath = PathString.Empty; // Disable redirects
        options.LogoutPath = PathString.Empty; // Disable redirects
        options.AccessDeniedPath = PathString.Empty; // Disable redirects

        // Handle redirects manually
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }
        };
    });

services.AddAuthorization();

services.AddDbContext<AirportDbContext>(
    options => options
        .UseNpgsql(config.GetConnectionString("AirportContextConnectionString"))
        .UseLazyLoadingProxies()
        .UseSnakeCaseNamingConvention());

services.AddTransient<IPasswordHasher, HMACSHA256PasswordHasher>();

services.AddScoped<IValidator<CreateUserRequest>, CreateUserRequestValidator>();
services.AddScoped<IValidator<CreateStaffRequest>, CreateStaffRequestValidator>();
services.AddScoped<IValidator<CreateFlightRequest>, CreateFlightRequestValidator>();
services.AddScoped<IValidator<CreateTicketProductRequest>, CreateTicketProductRequestValidator>();
services.AddScoped<IValidator<UpdateUserRequest>, UpdateUserRequestValidator>();

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
apiEndpoints.RegisterUserEndpoints();
apiEndpoints.RegisterRetailEndpoints();
apiEndpoints.RegisterStatisticsEndpoints();

app.Run();