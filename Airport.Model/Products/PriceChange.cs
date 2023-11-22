namespace Airport.Model.Products;

public class PriceChange
{
    public PriceChange(Guid id, DateTime changedAt, Product product, decimal price)
    {
        Id = id;
        ChangedAt = changedAt;
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
    
    public Product Product { get; private set; }
    
    public Guid ProductId { get; private set; }
    
    public decimal Price { get; private set; }
}