using Airport.Model.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Airport.Model;

public class AirportDbContext : DbContext
{
    public DbSet<User> Users { get; protected set; } = null!;

    public AirportDbContext(DbContextOptions<AirportDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
    }
}