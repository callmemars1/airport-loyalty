namespace Airport.Model.Flights;

public class RowClass
{
    public RowClass(
        Guid id, 
        string title,
        short serviceLevel,
        short seatsPerRow, 
        short rows,
        short rowsStartOffset,
        AirplaneModel model)
    {
        Id = id;
        Title = title;
        ServiceLevel = serviceLevel;
        SeatsPerRow = seatsPerRow;
        Rows = rows;
        RowsStartOffset = rowsStartOffset;
        ModelId = model.Id;
        Model = model;
    }

    // EF
    private RowClass()
    {
    }
    
    public Guid Id { get; private set; }
    
    public string Title { get; private set; } = null!;
    
    public short ServiceLevel { get; private set; }
    
    public short SeatsPerRow { get; private set; }
    
    public short Rows { get; private set; }
    
    public short RowsStartOffset { get; private set; }
    
    public int ModelId { get; private set; }

    public AirplaneModel Model { get; private set; } = null!;
}