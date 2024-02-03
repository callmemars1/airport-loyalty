using Airport.Model.Flights;
using Airport.Model.Products;
using Airport.Model.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;

namespace Airport.Model;

public class AirportDbContext : DbContext
{
    public DbSet<User> Users { get; protected set; } = null!;
    public DbSet<Role> Roles { get; protected set; } = null!;

    public DbSet<Flight> Flights { get; protected set; } = null!;
    public DbSet<Airplane> Airplanes { get; protected set; } = null!;
    public DbSet<AirplaneModel> AirplaneModels { get; protected set; } = null!;
    public DbSet<Airline> Airlines { get; protected set; } = null!;
    public DbSet<Gate> Gates { get; protected set; } = null!;
    public DbSet<Airport.Model.Flights.Airport> Airports { get; protected set; } = null!;
    public DbSet<RowClass> RowClasses { get; protected set; } = null!;

    public DbSet<Product> Products { get; protected set; } = null!;
    public DbSet<TicketProduct> TicketProducts { get; protected set; } = null!;
    public DbSet<PriceChange> PriceChanges { get; protected set; } = null!;
    public DbSet<Purchase> Purchases { get; protected set; } = null!;
    public DbSet<PurchasedProductBase> PurchasedProducts { get; protected set; } = null!;
    public DbSet<PurchasedTicket> PurchasedTickets { get; protected set; } = null!;

    public AirportDbContext(DbContextOptions<AirportDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .Entity<Role>()
            .Property(d => d.SystemName)
            .HasConversion(new EnumToStringConverter<Roles>());

        modelBuilder.Entity<Product>()
            .ToTable("products")
            .HasDiscriminator(b => b.Discriminator);

        modelBuilder.Entity<PurchasedProductBase>()
            .ToTable("purchased_products")
            .HasDiscriminator(b => b.Discriminator);

        modelBuilder.Entity<PurchasedTicket>()
            .ToTable("purchased_products")
            .HasDiscriminator(b => b.Discriminator).HasValue("Ticket");

        modelBuilder.Entity<RowClass>()
            .ToTable("rows_classes");
        
        modelBuilder.Entity<PriceChange>()
            .ToTable("price_changes")
            .Property(p => p.Price)
            .HasColumnType("double precision");


    }
}