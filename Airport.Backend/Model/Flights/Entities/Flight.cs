namespace Airport.Backend.Model.Flights;

public class Flight
{
    public int Id { get; set; }
    public int DepartureAirportId { get; set; }
    public DateTime DepartureDateTimeUtc { get; set; }
    public DateTime ArrivalDateTimeUtc { get; set; }
    public int ArrivalAirportId { get; set; }
    public int? AirplaneId { get; set; }
    public int? GateId { get; set; }
    public decimal Price { get; set; }
    public virtual Airport ArrivalAirport { get; set; } = null!;
    public virtual Airport DepartureAirport { get; set; } = null!;
    public virtual Airplane Airplane { get; set; } = null!;
    public virtual Gate Gate { get; set; } = null!;
}