using Airport.Model.Flights;

namespace Airport.Model.Products;

public class TicketProduct : Product
{
    public TicketProduct(
        Guid id,
        string title,
        string description,
        Flight flight,
        RowClass serviceClass,
        long? quantity = null) 
        : base(id, title, description, quantity)
    {
        FlightId = flight.Id;
        Flight = flight;
        ServiceClass = serviceClass;
    }

    // EF
    protected TicketProduct()
    {
    }
    
    public override string Discriminator { get; } = "TicketProduct";
    
    public Guid FlightId { get; private set; }
    
    public Flight Flight { get; private set; }
    
    public RowClass ServiceClass { get; private set; }
}