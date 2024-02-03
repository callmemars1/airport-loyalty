namespace Airport.Model.Products;

public abstract class PurchasedProductBase
{
    protected PurchasedProductBase(
        Guid id,
        Purchase purchase,
        PriceChange actualPrice,
        short? quantity = null)
    {
        Id = id;
        Quantity = quantity;
        Purchase = purchase;
        ActualPrice = actualPrice;
    }
    
    // EF
    protected PurchasedProductBase()
    {
        
    }
    
    public Guid Id { get; private set; }
    
    public short? Quantity { get; private set; }
    
    public virtual Purchase Purchase { get; private set; } = null!;

    public virtual PriceChange ActualPrice { get; private set; } = null!;
    
    public abstract string Discriminator { get; protected set; }
}

public abstract class PurchasedProduct<TProduct> : PurchasedProductBase
    where TProduct : Product
{
    protected PurchasedProduct(
        Guid id,
        Purchase purchase,
        PriceChange actualPriceChange,
        TProduct product,
        short? quantity = null) : base(id, purchase, actualPriceChange, quantity)
    {
        Product = product;
    }
    
    // EF
    protected PurchasedProduct()
    {
    }

    public virtual TProduct Product { get; private set; }
}