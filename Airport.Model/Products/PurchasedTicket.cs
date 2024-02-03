namespace Airport.Model.Products;

public class PurchasedTicket : PurchasedProduct<TicketProduct>
{
    public PurchasedTicket(
        Guid id,
        TicketProduct product,
        Purchase purchase,
        PriceChange actualPriceChange,
        string ticketIdentifier,
        short seatNumber,
        short? quantity = null) : base(id,
        purchase,
        actualPriceChange,
        product,
        quantity)
    {
        TicketNumber = ticketIdentifier;
        SeatNumber = seatNumber;
    }

    // EF
    protected PurchasedTicket()
    {
    }

    public override string Discriminator { get; protected set; } = "Ticket";

    public string TicketNumber { get; private set; } = null!;

    public short SeatNumber { get; private set; }
}