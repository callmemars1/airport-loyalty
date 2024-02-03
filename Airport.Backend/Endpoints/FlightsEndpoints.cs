using Airport.Backend.Utils;
using Airport.Model;
using Airport.Model.Flights;
using Airport.Model.Users;
using FluentValidation;
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
        flightsEndpointsGroup.MapGet("by-number", GetFlightByNumberAsync);
        flightsEndpointsGroup.MapGet("airlines", GetAirlines);
        flightsEndpointsGroup.MapPost("create", CreateFlight).RequireAuthorization(p =>
        {
            p.RequireClaim(AuthClaims.Role, Roles.Admin.ToString(), Roles.Editor.ToString());
        });
        flightsEndpointsGroup.MapPatch("cancel", CancelFlight).RequireAuthorization(p =>
        {
            p.RequireClaim(AuthClaims.Role, Roles.Admin.ToString(), Roles.Editor.ToString());
        });
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

    private static async Task<Results<Ok<TableFlightDto>, NotFound>> GetFlightByNumberAsync(
        [FromQuery] string flightNumber,
        AirportDbContext dbContext)
    {
        var flight = await dbContext.Flights.SingleOrDefaultAsync(f => f.FlightNumber == flightNumber);

        return flight is null
            ? TypedResults.NotFound()
            : TypedResults.Ok(TableFlightDto.Map(flight));
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

    private static async Task<Results<Created<TableFlightDto>, ValidationProblem>> CreateFlight(
        AirportDbContext dbContext,
        IValidator<CreateFlightRequest> validator,
        [FromBody] CreateFlightRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);
        
        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionaryLower());
        
        var departureAirport = await dbContext
            .Airports
            .FirstAsync(r => r.IataCode == request.DepartureAirportCode);

        var departureGate = await dbContext
            .Gates
            .Where(g => g.AirportId == departureAirport.Id)
            .OrderBy(_ => Guid.NewGuid())
            .FirstAsync();

        var arrivalAirport =
            await dbContext
                .Airports.FirstAsync(r => r.IataCode == request.ArrivalAirportCode);

        var arrivalGate = await dbContext
            .Gates
            .Where(g => g.AirportId == arrivalAirport.Id)
            .OrderBy(_ => Guid.NewGuid())
            .FirstAsync();

        var airline = await dbContext.Airlines.FirstAsync(a => a.Code == request.AirlineCode);

        var airplane = await dbContext
            .Airplanes
            .Where(a => a.AirlineId == airline.Id)
            .OrderBy(_ => Guid.NewGuid())
            .FirstAsync();

        var createdFlight = new Flight(
            Guid.NewGuid(),
            departureAirport,
            departureGate,
            DateTime.SpecifyKind(request.DepartureDateTime, DateTimeKind.Utc).AddHours(3),
            arrivalAirport,
            arrivalGate,
            request.DepartureDateTime.AddHours(Random.Shared.Next(1, 17)),
            airplane,
            $"{airline.Code}{Random.Shared.Next(0, 1000000):000000}");

        await dbContext.Flights.AddAsync(createdFlight);
        await dbContext.SaveChangesAsync();

        return TypedResults.Created($"api/flights/by-number?flightNumber={createdFlight.FlightNumber}",
            TableFlightDto.Map(createdFlight));
    }

    private static async Task<Results<Ok, NotFound>> CancelFlight(string flightNumber, AirportDbContext dbContext)
    { 
        var cancelled = await dbContext.Flights.Where(f => f.FlightNumber == flightNumber)
            .ExecuteUpdateAsync(x => x.SetProperty(f => f.Cancelled, _ => true));

        return cancelled == 0 ? TypedResults.NotFound() : TypedResults.Ok();
    }
    
    private static async Task<Ok<IEnumerable<AirlineDto>>> GetAirlines(AirportDbContext dbContext)
    {
        var airlines = await dbContext.Airlines.ToArrayAsync();
        return TypedResults.Ok(airlines.Select(AirlineDto.Map));
    }
}

public record CreateFlightRequest(
    string DepartureAirportCode,
    string ArrivalAirportCode,
    string AirlineCode,
    DateTime DepartureDateTime
);

public class CreateFlightRequestValidator : AbstractValidator<CreateFlightRequest>
{
    public CreateFlightRequestValidator(AirportDbContext dbContext)
    {
        RuleFor(x => x.AirlineCode)
            .NotEmpty().WithMessage("Не заполнено!")
            .NotNull().WithMessage("Не заполнено!")
            .MustAsync(async (code, ct)
                => await dbContext.Airlines.AnyAsync(a => a.Code == code, cancellationToken: ct))
            .WithMessage("Авиалиния не найдена!");

        RuleFor(x => x.DepartureAirportCode)
            .NotEmpty().WithMessage("Не заполнено!")
            .NotNull().WithMessage("Не заполнено!")
            .MustAsync(async (code, ct) => await dbContext.Airports.AnyAsync(a => a.IataCode == code, ct))
            .WithMessage("Аэропорт вылета не найден!");

        RuleFor(x => x.ArrivalAirportCode)
            .NotEmpty().WithMessage("Не заполнено!")
            .NotNull().WithMessage("Не заполнено!")
            .MustAsync(async (code, ct) => await dbContext.Airports.AnyAsync(a => a.IataCode == code, ct))
            .WithMessage("Аэропорт прилёта не найден!");

        RuleFor(x => x)
            .Must(x => x.DepartureAirportCode != x.ArrivalAirportCode)
            .WithName("arrivalAirportCode")
            .WithMessage("Аэропортом прилета не может быть аэропорт вылета!");

        RuleFor(x => x.DepartureDateTime)
            .NotEmpty().WithMessage("Не заполнено!")
            .NotNull().WithMessage("Не заполнено!")
            .Must(x => x >= DateTime.UtcNow)
            .WithMessage("Рейс не может быть создан в прошлом!");
    }
}

public record TableFlightDto(
    Guid Id,
    AirportDto ArrivalAirport,
    GateDto ArrivalGate,
    DateTime ArrivalDateTime,
    AirportDto DepartureAirport,
    GateDto DepartureGate,
    DateTime DepartureDateTime,
    AirlineDto Airline,
    string FlightNumber,
    bool Cancelled
)
{
    public static TableFlightDto Map(Flight flight) =>
        new(
            Id: flight.Id,
            ArrivalAirport: AirportDto.Map(flight.ArrivalAirport),
            ArrivalGate: GateDto.Map(flight.ArrivalGate),
            ArrivalDateTime: flight.ArrivalDateTimeUtc,
            DepartureAirport: AirportDto.Map(flight.DepartureAirport),
            DepartureGate: GateDto.Map(flight.DepartureGate),
            DepartureDateTime: flight.DepartureDateTimeUtc,
            Airline: AirlineDto.Map(flight.Airplane.Airline),
            FlightNumber: flight.FlightNumber, //$"{flight.Airplane.Airline.Code}-{flight.Id.ToString()[..6]}"
            Cancelled: flight.Cancelled
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