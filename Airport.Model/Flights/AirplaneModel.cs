namespace Airport.Model.Flights;

public class AirplaneModel
{
    public AirplaneModel(int id, string title, string manufacturer)
    {
        Id = id;
        Title = title;
        Manufacturer = manufacturer;
    }

    // EF
    protected AirplaneModel()
    {
    }

    public int Id { get; private set; }

    public string Title { get; private set; } = null!;

    public string Manufacturer { get; private set; } = null!;

    public virtual IEnumerable<RowClass> RowClasses { get; private set; } = Array.Empty<RowClass>();
}