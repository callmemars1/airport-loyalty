namespace Airport.Backend.Model.Flights;

public static class FlightsExtensions
{
    public static IServiceCollection RegisterFlights(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddDbContext<FlightsDbContext>();
        return serviceCollection;
    }
}