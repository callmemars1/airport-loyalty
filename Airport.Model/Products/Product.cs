namespace Airport.Model.Products;

public abstract class Product
{
    protected Product(Guid id, string title, string description, long? quantity = null)
    {
        Id = id;
        Title = title;
        Description = description;
        Quantity = quantity;
    }

    // EF
    protected Product()
    {
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = null!;

    public string Description { get; private set; } = null!;

    public long? Quantity { get; private set; }
    
    public abstract string Discriminator { get; }
}