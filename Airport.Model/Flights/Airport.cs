namespace Airport.Model.Flights;

public class Airport
{
    public Airport(int id, string code, string city, string country)
    {
        Id = id;
        Code = code;
        City = city;
        Country = country;
    }

    // EF
    private Airport()
    {
    }

    public int Id { get; private set; }
    
    public string Code { get; private set; } = null!;
    
    public string City { get; private set; } = null!;
    
    public string Country { get; private set; } = null!;

    public IEnumerable<Airplane> Airplanes { get; private set; } = new List<Airplane>();

    public IEnumerable<Gate> Gates { get; private set; } = new List<Gate>();
}