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
        RowsCount = rows;
        RowsOffset = rowsStartOffset;
        AirplaneModelId = model.Id;
        AirplaneModel = model;
    }

    // EF
    protected RowClass()
    {
    }
    
    public Guid Id { get; private set; }
    
    public string Title { get; private set; } = null!;
    
    public short ServiceLevel { get; private set; }
    
    public short RowsCount { get; private set; }
    
    public short RowsOffset { get; private set; }
    
    public int AirplaneModelId { get; private set; }
    
    public short SeatsPerRow { get; private set; }

    public virtual AirplaneModel AirplaneModel { get; private set; } = null!;
}