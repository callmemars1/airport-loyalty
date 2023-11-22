using Airport.Model.Users;

namespace Airport.Model.Products;

public class Transaction
{
    public Transaction(Guid id, DateTime createdAt, User user, decimal totalPrice)
    {
        Id = id;
        CreatedAt = createdAt;
        User = user;
        UserId = user.Id;
        TotalPrice = totalPrice;
    }

    // EF
    protected Transaction()
    {
    }

    public Guid Id { get; private set; }
    
    public DateTime CreatedAt { get; private set; }

    public User User { get; private set; } = null!;
    
    public Guid UserId { get; private set; }

    public IEnumerable<PurchasedProduct> PurchasedProducts { get; private set; } = new List<PurchasedProduct>();
    
    public decimal TotalPrice { get; private set; }
}