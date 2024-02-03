using Airport.Model.Flights;
using Airport.Model.Users;

namespace Airport.Model.Products;

public class TicketProduct : Product
{
    public TicketProduct(
        Guid id,
        string title,
        User createdBy,
        Flight flight,
        RowClass serviceClass,
        long? quantity = null) 
        : base(id, title, createdBy, quantity)
    {
        FlightId = flight.Id;
        Flight = flight;
        ServiceClass = serviceClass;
    }

    // EF
    protected TicketProduct()
    {
    }
    
    public override string Discriminator { get; protected set; } = "TicketProduct";
    
    public Guid FlightId { get; private set; }
    
    public virtual Flight Flight { get; private set; }
    
    public virtual RowClass ServiceClass { get; private set; }
}