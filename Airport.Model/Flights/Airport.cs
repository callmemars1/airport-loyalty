namespace Airport.Model.Flights;

public class Airport
{
    public Airport(int id, string iataCode, string city, string country, string name)
    {
        Id = id;
        IataCode = iataCode;
        City = city;
        Country = country;
        Name = name;
    }

    // EF
    protected Airport()
    {
    }

    public int Id { get; private set; }
    
    public string IataCode { get; private set; } = null!;

    public string Name { get; private set; } = null!;
    
    public string City { get; private set; } = null!;
    
    public string Country { get; private set; } = null!;

    public virtual IEnumerable<Airplane> Airplanes { get; private set; } = new List<Airplane>();

    public virtual IEnumerable<Gate> Gates { get; private set; } = new List<Gate>();
}