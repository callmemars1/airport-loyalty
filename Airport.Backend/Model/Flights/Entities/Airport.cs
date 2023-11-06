namespace Airport.Backend.Model.Flights;

public class Airport
{
    public int Id { get; set; }
    public string Code { get; set; } = null!;
    public string City { get; set; } = null!;
    public string Country { get; set; } = null!;
}