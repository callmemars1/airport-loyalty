namespace Airport.Model.Flights;

public class Flight
{
    public Flight(
        Guid id,
        Airport departureAirport,
        Gate departureGate,
        DateTime departureDateTimeUtc,
        Airport arrivalAirport,
        Gate arrivalGate,
        DateTime arrivalDateTimeUtc,
        Airplane airplane)
    {
        Id = id;
        DepartureAirport = departureAirport;
        DepartureAirportId = departureAirport.Id;
        DepartureGate = departureGate;
        DepartureGateId = departureGate.Id;
        DepartureDateTimeUtc = departureDateTimeUtc;

        ArrivalAirport = arrivalAirport;
        ArrivalAirportId = arrivalAirport.Id;
        ArrivalGate = arrivalGate;
        ArrivalGateId = arrivalGate.Id;
        ArrivalDateTimeUtc = arrivalDateTimeUtc;
        Airplane = airplane;
        AirplaneId = airplane.Id;
    }

    protected Flight()
    {
    } // EF

    public Guid Id { get; private set; }

    public int DepartureAirportId { get; private set; }

    public virtual Airport DepartureAirport { get; private set; } = null!;

    public short? DepartureGateId { get; private set; }

    public virtual Gate DepartureGate { get; private set; } = null!;

    public DateTime DepartureDateTimeUtc { get; private set; }


    public int ArrivalAirportId { get; private set; }

    public virtual Airport ArrivalAirport { get; private set; } = null!;

    public short? ArrivalGateId { get; private set; }

    public virtual Gate ArrivalGate { get; private set; } = null!;

    public DateTime ArrivalDateTimeUtc { get; private set; }


    public int? AirplaneId { get; private set; }

    public virtual Airplane Airplane { get; private set; } = null!;
}