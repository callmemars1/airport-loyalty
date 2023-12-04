namespace Airport.Model.Flights;

public class Gate
{
    public Gate(short id, string name, string terminal, Airport airport)
    {
        Id = id;
        Name = name;
        Terminal = terminal;
        Airport = airport;
        AirportId = airport.Id;
    }

    // EF
    protected Gate()
    {
    }

    public short Id { get; set; }
    
    public string Name { get; set; } = null!;
    
    public string Terminal { get; set; } = null!;

    public virtual Airport Airport { get; private set; } = null!;
    
    public int AirportId { get; private set; }
}