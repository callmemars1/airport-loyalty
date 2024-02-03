using Airport.Model.Users;

namespace Airport.Model.Products;

public class PriceChange
{
    public PriceChange(Guid id, DateTime changedAt, User changedBy, Product product, double price)
    {
        Id = id;
        ChangedAt = changedAt;
        ChangedBy = changedBy;
        Product = product;
        ProductId = product.Id;
        Price = price;
    }

    // EF
    protected PriceChange()
    {
    }
    
    public Guid Id { get; private set; }

    public DateTime ChangedAt { get; private set; }
    public virtual User ChangedBy { get; private set; }
    
    public virtual Product Product { get; private set; }
    
    public Guid ProductId { get; private set; }
    
    public double Price { get; private set; }
}