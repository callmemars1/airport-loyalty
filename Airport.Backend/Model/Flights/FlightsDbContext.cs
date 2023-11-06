using Microsoft.EntityFrameworkCore;

namespace Airport.Backend.Model.Flights;

public class FlightsDbContext : DbContext
{
    public DbSet<Gate> Gates { get; protected set; } = null!;
    public DbSet<Airport> Airports { get; protected set; } = null!;
    public DbSet<Airline> Airlines { get; protected set; } = null!;
    public DbSet<Airplane> Airplanes { get; protected set; } = null!;
    public DbSet<Flight> Flights { get; protected set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder
            .UseNpgsql("Host=localhost;Database=airport;Username=airport-backend;Password=airport-password")
            .UseLazyLoadingProxies()
            .UseSnakeCaseNamingConvention();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Gate>()
            .HasIndex(g => new { g.Name, g.Terminal })
            .IsUnique();
        
        modelBuilder.Entity<Airport>()
            .HasIndex(a => a.Code)
            .IsUnique();

        modelBuilder.Entity<Airport>()
            .HasIndex(a => new { a.Country, a.City })
            .IsUnique();

        modelBuilder.Entity<Airline>()
            .HasIndex(a => a.Code)
            .IsUnique();
    }
}