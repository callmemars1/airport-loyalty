namespace Airport.Model.Flights;

public class Airline
{
    public Airline(short id, string name, string code)
    {
        Id = id;
        Name = name;
        Code = code;
    }

    // EF
    private Airline()
    {
    }

    public short Id { get; private set; }

    public string Name { get; private set; } = null!;

    public string Code { get; private set; } = null!;
}