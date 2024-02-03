using System.Security.Claims;
using System.Text.Json;
using Airport.Backend.Utils;
using Airport.Model;
using Airport.Model.Users;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Airport.Backend.Endpoints;

public static class StatisticsEndpoints
{
    public static void RegisterStatisticsEndpoints(this IEndpointRouteBuilder app)
    {
        var statisticsEndpointsGroup = app.MapGroup("/statistics");

        statisticsEndpointsGroup.MapGet("/users-per-date", GetUserCreationPerDayStatistics).RequireAuthorization(p =>
        {
            p.RequireClaim(AuthClaims.Role, Roles.Admin.ToString(), Roles.Editor.ToString(), Roles.Analyst.ToString());
        });
        
        statisticsEndpointsGroup.MapGet("/flights-per-date", GetFlightsPerDay).RequireAuthorization(p =>
        {
            p.RequireClaim(AuthClaims.Role, Roles.Admin.ToString(), Roles.Editor.ToString(), Roles.Analyst.ToString());
        });

        statisticsEndpointsGroup.MapGet("/purchases-per-date", GetPurchasesPerDay).RequireAuthorization(p =>
        {
            p.RequireClaim(AuthClaims.Role, Roles.Admin.ToString(), Roles.Editor.ToString(), Roles.Analyst.ToString());
        });
    }
    
    

    private static async Task<Ok<PerDayStatistics>> GetUserCreationPerDayStatistics(
        AirportDbContext dbContext,
        ClaimsPrincipal userClaims)
    {
        var usersCountPerDate = (await dbContext.Users
            .GroupBy(user => user.CreatedAt.Date) // Group by the date part of CreationDate
            .ToArrayAsync())
            .Select(group =>
                new PerDateStatisticUnit(DateOnly.FromDateTime(group.Key), group.Count()))
            .OrderBy(x => x.Date)
            .ToArray();

        return TypedResults.Ok(new PerDayStatistics(usersCountPerDate));
    }
    
    private static async Task<Ok<PerDayStatistics>> GetFlightsPerDay(
        AirportDbContext dbContext,
        ClaimsPrincipal userClaims)
    {
        var countPerDate = (await dbContext.Flights
                .Where(f => f.DepartureDateTimeUtc >= DateTime.UtcNow.AddDays(-50))
                .Where(f => f.DepartureDateTimeUtc <= DateTime.UtcNow.AddDays(50))
                .OrderByDescending(f => f.DepartureDateTimeUtc)
                .GroupBy(f => f.DepartureDateTimeUtc.Date) // Group by the date part of CreationDate
                .ToArrayAsync())
            .Select(group =>
                new PerDateStatisticUnit(DateOnly.FromDateTime(group.Key), group.Count()))
            .OrderBy(x => x.Date)
            .ToArray();

        return TypedResults.Ok(new PerDayStatistics(countPerDate));
    }
    
    private static async Task<Ok<PerDayStatistics>> GetPurchasesPerDay(
        AirportDbContext dbContext,
        ClaimsPrincipal userClaims)
    {
        var countPerDate = (await dbContext.Purchases
                .OrderByDescending(f => f.CreatedAt)
                .GroupBy(f => f.CreatedAt.Date) // Group by the date part of CreationDate
                .ToArrayAsync())
            .Select(group =>
                new PerDateStatisticUnit(DateOnly.FromDateTime(group.Key), group.Count()))
            .OrderBy(x => x.Date)
            .ToArray();

        return TypedResults.Ok(new PerDayStatistics(countPerDate));
    }


  
}

public record PerDayStatistics(IEnumerable<PerDateStatisticUnit> Units);

public record PerDateStatisticUnit(DateOnly Date, long UnitsCount);