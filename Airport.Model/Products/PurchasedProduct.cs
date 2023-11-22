namespace Airport.Model.Products;

public abstract class PurchasedProduct
{
    protected PurchasedProduct(
        Guid id,
        Product product,
        Transaction transaction,
        PriceChange actualPriceChange,
        short? quantity = null)
    {
        Id = id;
        Product = product;
        ProductId = product.Id;
        Quantity = quantity;
        Transaction = transaction;
        TransactionId = transaction.Id;
        ActualPriceChange = actualPriceChange;
        ActualPriceChangeId = actualPriceChange.Id;
    }
    
    // EF
    protected PurchasedProduct()
    {
    }

    public Guid Id { get; private set; }
    
    public Product Product { get; private set; }
    
    public Guid ProductId { get; private set; }
    
    public short? Quantity { get; private set; }
    
    public Guid TransactionId { get; private set; }

    public Transaction Transaction { get; private set; } = null!;

    public PriceChange ActualPriceChange { get; private set; } = null!;
    
    public Guid ActualPriceChangeId { get; private set; }
    
    public abstract string Discriminator { get; }
}