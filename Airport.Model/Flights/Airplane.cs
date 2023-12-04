namespace Airport.Model.Flights;

public class Airplane
{
    public Airplane(
        int id,
        Airline airline,
        AirplaneModel model,
        Airport airport)
    {
        Id = id;
        AirlineId = airline.Id;
        Airline = airline;
        ModelId = model.Id;
        Model = model;
        AirportId = airport.Id;
        Airport = airport;
    }
    
    // EF
    protected Airplane()
    {
    }

    public int Id { get; private set; }

    public short AirlineId { get; private set; }

    public virtual Airline Airline { get; private set; } = null!;
    
    public int ModelId { get; private set; }

    public virtual AirplaneModel Model { get; private set; } = null!;
    
    public int AirportId { get; private set; }

    public virtual Airport Airport { get; private set; } = null!;
}