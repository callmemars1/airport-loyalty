/*using Airport.Backend.Model.Core;
using Airport.Backend.Model.Flights;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Airport.Backend.Endpoints;

using GetFlightByIdResult = Results<Ok<FlightDto>, NotFound>;

public static class FlightsEndpoints
{
    public static void RegisterFlightsEndpoints(this WebApplication app)
    {
        var flightsEndpointsGroup = app.MapGroup("/flights");

        flightsEndpointsGroup.MapGet("", GetFlightByIdAsync);
        flightsEndpointsGroup.MapGet("arriving", GetArrivingFlightsAsync);
        flightsEndpointsGroup.MapGet("departing", GetDepartingFlightsAsync);
        flightsEndpointsGroup.MapPost("search", GetFlightsBySearchParameters);
    }

    private static async Task<GetFlightByIdResult> GetFlightByIdAsync(
        [FromQuery] Guid flightId,
        FlightsDbContext dbContext)
        => await dbContext.Flights.Where(f => f.Id == flightId).SingleOrDefaultAsync() is { } flight
            ? TypedResults.Ok(FlightDto.CreateFromEntity(flight))
            : TypedResults.NotFound();

    private static async Task<Ok<IEnumerable<FlightDto>>> GetArrivingFlightsAsync(
        [FromQuery] string atAirportIata,
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? tillUtc,
        FlightsDbContext dbContext)
    {
        var flights = await dbContext.Flights
            .Where(f => f.ArrivalAirport.Code == atAirportIata)
            .Where(f => fromUtc == null || fromUtc.Value.AsUtc() <= f.ArrivalDateTimeUtc)
            .Where(f => tillUtc == null || tillUtc.Value.AsUtc() >= f.ArrivalDateTimeUtc)
            .ToListAsync();

        var dtos = flights.Select(FlightDto.CreateFromEntity);
        return TypedResults.Ok(dtos);
    }

    private static async Task<Ok<IEnumerable<FlightDto>>> GetDepartingFlightsAsync(
        [FromQuery] string fromAirportIata,
        [FromQuery] DateTime? fromUtc,
        [FromQuery] DateTime? tillUtc,
        FlightsDbContext dbContext)
    {
        var flights = await dbContext.Flights
            .Where(f => f.DepartureAirport.Code == fromAirportIata)
            .Where(f => fromUtc == null || fromUtc.Value.AsUtc() <= f.DepartureDateTimeUtc)
            .Where(f => tillUtc == null || tillUtc.Value.AsUtc() >= f.DepartureDateTimeUtc)
            .ToListAsync();

        var dtos = flights.Select(FlightDto.CreateFromEntity);
        return TypedResults.Ok(dtos);
    }
    
    private static async Task<Ok<IEnumerable<FlightDto>>> GetFlightsBySearchParameters(
        [FromBody] FlightSearchParametersDto searchParameters,
        FlightsDbContext dbContext)
    {
        var fromUtc = searchParameters.FromUtc?.AsUtc() ?? DateTime.UtcNow;
        var tillUtc = searchParameters.TillUtc?.AsUtc();
        var flights = await dbContext.Flights
            .Where(f => fromUtc <= f.DepartureDateTimeUtc)
            .Where(f => tillUtc == null || tillUtc.Value.AsUtc() >= f.DepartureDateTimeUtc)
            .Where(f => f.ArrivalAirportId == searchParameters.ArrivalAirportId)
            .Where(f => f.DepartureAirportId == searchParameters.DepartureAirportId)
            .OrderBy(f => f.DepartureDateTimeUtc)
            .ToListAsync();

        var dtos = flights.Select(FlightDto.CreateFromEntity);
        return TypedResults.Ok(dtos);
    }
}*/