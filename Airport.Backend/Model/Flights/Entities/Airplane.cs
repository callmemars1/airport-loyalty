namespace Airport.Backend.Model.Flights;

public class Airplane
{
    public int Id { get; set; }
    public int AirlineId { get; set; }
    public int Seats { get; set; }
    public string Model { get; set; } = null!;
    public virtual Airline Airline { get; set; } = null!;
}