/*using Airport.Model.Flights;

namespace Airport.Backend.Endpoints;

public record FlightDto(
    Guid Id,
    AirportDto DepartureAirport,
    AirportDto ArrivalAirport,
    DateTime DepartureDateTimeUtc,
    DateTime ArrivalDateTimeUtc,
    AirplaneDto Airplane,
    GateDto Gate
)
{
    public static FlightDto CreateFromEntity(Flight flight)
        => new(
            flight.Id,
            AirportDto.CreateFromEntity(flight.DepartureAirport),
            AirportDto.CreateFromEntity(flight.ArrivalAirport),
            flight.DepartureDateTimeUtc,
            flight.ArrivalDateTimeUtc,
            AirplaneDto.CreateFromEntity(flight.Airplane),
            GateDto.CreateFromEntity(flight.ArrivalGate)
        );
}

public record AirportDto(
    int Id,
    string Code,
    string City,
    string Country
)
{
    public static AirportDto CreateFromEntity(Model.Flights.Airport airport)
        => new(
            airport.Id,
            airport.Code,
            airport.City,
            airport.Country
        );
}

public record AirplaneDto(
    int Id,
    AirlineDto Airline,
    int Seats,
    string Model
)
{
    public static AirplaneDto CreateFromEntity(Airplane airplane)
        => new(
            airplane.Id,
            AirlineDto.CreateFromEntity(airplane.Airline),
            airplane.Seats,
            airplane.Model
        );
}

public record AirlineDto(
    int Id,
    string Name,
    string Code
)
{
    public static AirlineDto CreateFromEntity(Airline airline)
        => new(
            airline.Id,
            airline.Name,
            airline.Code
        );
}

public record GateDto(
    int Id,
    string Name,
    string Terminal
)
{
    public static GateDto CreateFromEntity(Gate gate)
        => new(
            gate.Id,
            gate.Name,
            gate.Terminal
        );
}

public record FlightSearchParametersDto(
    DateTime? FromUtc,
    DateTime? TillUtc,
    int DepartureAirportId,
    int ArrivalAirportId);*/