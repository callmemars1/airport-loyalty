namespace Airport.Backend.Model.Flights;

public class Gate
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Terminal { get; set; } = null!;
}