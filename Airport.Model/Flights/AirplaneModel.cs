namespace Airport.Model.Flights;

public class AirplaneModel
{
    public AirplaneModel(int id, string title, string brand)
    {
        Id = id;
        Title = title;
        Brand = brand;
    }

    // EF
    private AirplaneModel()
    {
    }

    public int Id { get; private set; }

    public string Title { get; private set; } = null!;

    public string Brand { get; private set; } = null!;

    public IEnumerable<RowClass> RowClasses { get; private set; } = Array.Empty<RowClass>();
}