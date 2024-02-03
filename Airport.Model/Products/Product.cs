using Airport.Model.Users;

namespace Airport.Model.Products;

public abstract class Product
{
    protected Product(Guid id, string title, User createdBy, long? quantity = null, bool deleted = false)
    {
        Id = id;
        Title = title;
        CreatedBy = createdBy;
        Quantity = quantity;
        Deleted = deleted;
    }

    // EF
    protected Product()
    {
    }

    public Guid Id { get; private set; }

    public string Title { get; private set; } = null!;

    public virtual User CreatedBy { get; private set; } = null!;

    public long? Quantity { get; private set; }
    
    public abstract string Discriminator { get; protected set; }

    public bool Deleted { get; set; } = false;

    public virtual IEnumerable<PriceChange> PriceChanges { get; private set; } = new List<PriceChange>();

    public PriceChange ActualPrice => PriceChanges
        .OrderByDescending(p => p.ChangedAt)
        .First();
}