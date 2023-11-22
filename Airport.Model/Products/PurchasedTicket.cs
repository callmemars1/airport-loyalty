namespace Airport.Model.Products;

class PurchasedTicket : PurchasedProduct
{
    public PurchasedTicket(
        Guid id,
        Product product,
        Transaction transaction,
        PriceChange actualPriceChange, 
        string ticketIdentifier,
        short seatNumber,
        short? quantity = null) : base(id,
        product,
        transaction,
        actualPriceChange,
        quantity)
    {
        TicketIdentifier = ticketIdentifier;
        SeatNumber = seatNumber;
    }

    // EF
    protected PurchasedTicket()
    {
    }

    public override string Discriminator { get; } = "Ticket";

    public string TicketIdentifier { get; private set; } = null!;
    
    public short SeatNumber { get; private set; }
}