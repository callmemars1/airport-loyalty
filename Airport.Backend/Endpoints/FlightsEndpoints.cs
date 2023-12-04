using Airport.Model;
using Airport.Model.Flights;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Airport.Backend.Endpoints;


public static class FlightsEndpoints
{
    public static void RegisterFlightsEndpoints(this IEndpointRouteBuilder app)
    {
        var flightsEndpointsGroup = app.MapGroup("/flights");

        flightsEndpointsGroup.MapGet("arriving", GetArrivingFlightsAsync);
        flightsEndpointsGroup.MapGet("departing", GetDepartingFlightsAsync);
        flightsEndpointsGroup.MapGet("airports", GetAirportsAsync);
        flightsEndpointsGroup.MapGet("filtered", GetFilteredFlightsAsync);
    }

    private static async Task<Ok<IEnumerable<TableFlightDto>>> GetArrivingFlightsAsync(
        [FromQuery] string atAirportIataCode,
        [FromQuery] DateTime? fromUtc,
        AirportDbContext dbContext)
    {
       var arrivingFlights = await dbContext.Flights
            .Where(f => f.ArrivalAirport.IataCode == atAirportIataCode)
            .Where(f => f.ArrivalDateTimeUtc >= fromUtc)
            .OrderBy(f => f.ArrivalDateTimeUtc)
            .Take(10)
            .ToListAsync();

       return TypedResults.Ok(arrivingFlights.Select(TableFlightDto.Map));
    }
    
    private static async Task<Ok<IEnumerable<TableFlightDto>>> GetDepartingFlightsAsync(
        [FromQuery] string atAirportIataCode,
        [FromQuery] DateTime? fromUtc,
        AirportDbContext dbContext)
    {
        var departingFlights = await dbContext.Flights
            .Where(f => f.DepartureAirport.IataCode == atAirportIataCode)
            .Where(f => f.DepartureDateTimeUtc >= fromUtc)
            .OrderBy(f => f.DepartureDateTimeUtc)
            .Take(10)
            .ToListAsync();

        return TypedResults.Ok(departingFlights.Select(TableFlightDto.Map));
    }
    
    private static async Task<Ok<IEnumerable<TableFlightDto>>> GetFilteredFlightsAsync(
        AirportDbContext dbContext,
        [FromQuery] string departureAirportCode,
        [FromQuery] string arrivalAirportCode,
        [FromQuery] DateTime? departureDate,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var departureDateMin = departureDate.HasValue
            ? departureDate.Value.ToUniversalTime().Date
            : DateTime.UtcNow.Date;
 
        var filteredFlights = await dbContext.Flights
            .Where(f => f.DepartureAirport.IataCode == departureAirportCode)
            .Where(f => f.ArrivalAirport.IataCode == arrivalAirportCode)
            .Where(f => f.DepartureDateTimeUtc.Date >= departureDateMin)
            .OrderBy(f => f.DepartureDateTimeUtc)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return TypedResults.Ok(filteredFlights.Select(TableFlightDto.Map));
    }

    private static async Task<Ok<IEnumerable<AirportDto>>> GetAirportsAsync(AirportDbContext dbContext)
    {
        var airports = await dbContext.Airports.OrderBy(a => a.IataCode).ToArrayAsync();
        return TypedResults.Ok(airports.Select(AirportDto.Map));
    }
}

public record TableFlightDto(
    AirportDto ArrivalAirport,
    GateDto ArrivalGate,
    DateTime ArrivalDateTime,
    AirportDto DepartureAirport,
    GateDto DepartureGate, 
    DateTime DepartureDateTime,
    AirlineDto Airline,
    string FlightNumber
)
{
    public static TableFlightDto Map(Flight flight) =>
        new(
            ArrivalAirport: AirportDto.Map(flight.ArrivalAirport),
            ArrivalGate: GateDto.Map(flight.ArrivalGate),
            ArrivalDateTime: flight.ArrivalDateTimeUtc,
            DepartureAirport: AirportDto.Map(flight.DepartureAirport),
            DepartureGate: GateDto.Map(flight.DepartureGate),
            DepartureDateTime: flight.DepartureDateTimeUtc,
            Airline: AirlineDto.Map(flight.Airplane.Airline),
            FlightNumber: $"{flight.Airplane.Airline.Code}-{flight.Id.ToString()[..6]}"
        );
}

public record AirportDto(string City, string Code)
{
    public static AirportDto Map(Model.Flights.Airport airport) => new(City: airport.City, Code: airport.IataCode);
}

public record GateDto(string Terminal, string Name)
{
    public static GateDto Map(Gate gate) => new(Terminal: gate.Terminal, Name: gate.Name);
}

public record AirlineDto(string Title, string Code)
{
    public static AirlineDto Map(Airline airline) => new(Title: airline.Name, Code: airline.Code);
}