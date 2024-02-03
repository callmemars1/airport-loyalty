using Airport.Model.Users;

namespace Airport.Model.Products;

public class Purchase
{
    public Purchase(Guid id, DateTime createdAt, User user, double totalPrice)
    {
        Id = id;
        CreatedAt = createdAt;
        User = user;
        UserId = user.Id;
        TotalPrice = totalPrice;
    }

    // EF
    protected Purchase()
    {
    }

    public Guid Id { get; private set; }
    
    public DateTime CreatedAt { get; private set; }

    public virtual User User { get; private set; } = null!;
    
    public Guid UserId { get; private set; }

    public virtual IEnumerable<PurchasedProductBase> PurchasedProducts { get; private set; } = new List<PurchasedProductBase>();
    
    public double TotalPrice { get; private set; }
}