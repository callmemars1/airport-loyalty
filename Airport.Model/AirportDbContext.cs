using Airport.Model.Flights;
using Airport.Model.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Airport.Model;

public class AirportDbContext : DbContext
{
    public DbSet<User> Users { get; protected set; } = null!;

    public DbSet<Flight> Flights { get; protected set; } = null!;
    public DbSet<Airplane> Airplanes { get; protected set; } = null!;
    public DbSet<AirplaneModel> AirplaneModels { get; protected set; } = null!;
    public DbSet<Airline> Airlines { get; protected set; } = null!;
    public DbSet<Gate> Gates { get; protected set; } = null!;
    public DbSet<Airport.Model.Flights.Airport> Airports { get; protected set; } = null!;
    public DbSet<RowClass> RowClasses { get; protected set; } = null!;

    public AirportDbContext(DbContextOptions<AirportDbContext> options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
    }
}