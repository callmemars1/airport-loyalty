using System.Security.Claims;
using Airport.Backend.Utils;
using Airport.Model;
using Airport.Model.Flights;
using Airport.Model.Products;
using Airport.Model.Users;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Airport.Backend.Endpoints;

public static class RetailEndpoints
{
    public static void RegisterRetailEndpoints(this IEndpointRouteBuilder app)
    {
        var retailFlightsEndpoints = app.MapGroup("/flights");
        retailFlightsEndpoints.MapPost("/create-product", CreateFlightProduct).RequireAuthorization(p =>
        {
            p.RequireClaim(AuthClaims.Role, Roles.Admin.ToString(), Roles.Editor.ToString());
        });
        retailFlightsEndpoints.MapGet("/service-classes", GetRowClassesByFlightId).RequireAuthorization(p =>
        {
            p.RequireClaim(AuthClaims.Role, Roles.Admin.ToString(), Roles.Editor.ToString());
        });

        var retailProductsEndpoints = app.MapGroup("/products");
        retailProductsEndpoints.MapGet("/", GetProducts);
        retailProductsEndpoints.MapDelete("/delete", DeleteProduct).RequireAuthorization(p =>
        {
            p.RequireClaim(AuthClaims.Role, Roles.Admin.ToString(), Roles.Editor.ToString());
        });
        retailProductsEndpoints.MapPatch("/change-price", ChangePrice).RequireAuthorization(p =>
        {
            p.RequireClaim(AuthClaims.Role, Roles.Admin.ToString(), Roles.Editor.ToString());
        });
        retailProductsEndpoints.MapPost("/buy-ticket", BuyTicket).RequireAuthorization();
        retailProductsEndpoints.MapGet("/get-tickets", GetBoughtTickets).RequireAuthorization();
        retailProductsEndpoints.MapGet("/tickets", GetFilteredFlightProducts).RequireAuthorization();
        retailProductsEndpoints.MapGet("/tickets-by-flight", GetFilteredFlightProductsByFlightNumber)
            .RequireAuthorization();
    }

    private static async Task<Ok<IEnumerable<TicketProductDto>>> GetFilteredFlightProductsByFlightNumber(
        AirportDbContext dbContext,
        string flightNumber)
    {
        var filteredFlights = dbContext.Flights
            .Where(f => f.FlightNumber == flightNumber);

        var filteredProducts = await dbContext.Products.OfType<TicketProduct>()
            .Where(p => filteredFlights.Any(f => f.Id == p.FlightId))
            .ToListAsync();

        return TypedResults.Ok(filteredProducts.Select(p => TicketProductDto.Map(p, dbContext)));
    }

    private static async Task<Ok<IEnumerable<TicketProductDto>>> GetFilteredFlightProducts(
        AirportDbContext dbContext,
        [FromQuery] string departureAirportCode,
        [FromQuery] string arrivalAirportCode,
        [FromQuery] DateTime? departureDate,
        [FromQuery] double? maxPrice,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var departureDateMin = departureDate.HasValue
            ? departureDate.Value.ToUniversalTime().Date
            : DateTime.UtcNow.Date;

        var depCode = departureAirportCode.Trim();
        var arrCode = arrivalAirportCode.Trim();

        var filteredFlights = dbContext.Flights
            .Where(f => f.DepartureAirport.IataCode == depCode.Trim())
            .Where(f => f.ArrivalAirport.IataCode == arrCode.Trim())
            .Where(f => f.DepartureDateTimeUtc.Date >= departureDateMin);

        var filteredProducts = await dbContext.Products.OfType<TicketProduct>()
            .Where(p => !p.Deleted)
            .Where(p => filteredFlights.Any(f => f.Id == p.FlightId))
            .Where(p => p.PriceChanges.OrderByDescending(pr => pr.ChangedAt).First().Price <=
                        (maxPrice ?? double.MaxValue))
            .OrderBy(p => p.Flight.DepartureDateTimeUtc).ThenBy(p => p.FlightId)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return TypedResults.Ok(filteredProducts.Select(p => TicketProductDto.Map(p, dbContext)));
    }

    private static async Task<Ok<IEnumerable<PurchasedTicketDto>>> GetBoughtTickets(
        AirportDbContext dbContext,
        ClaimsPrincipal userClaims)
    {
        var user = await GetUserByClaims(dbContext, userClaims);
        var tickets = await dbContext
            .PurchasedProducts
            .OfType<PurchasedTicket>()
            .Where(t => t.Purchase.UserId == user.Id)
            .ToArrayAsync();

        return TypedResults.Ok(tickets.Select(PurchasedTicketDto.Map));
    }

    private static async Task<Results<Ok<IEnumerable<PurchasedTicketDto>>, BadRequest<IDictionary<string, string[]>>, NotFound>> BuyTicket(
        AirportDbContext dbContext,
        ClaimsPrincipal userClaims,
        Guid productId,
        int quantity)
    {
        var user = await GetUserByClaims(dbContext, userClaims);

        var product = await dbContext.Products
            .OfType<TicketProduct>()
            .Include(ticketProduct => ticketProduct.Flight)
            .Include(ticketProduct => ticketProduct.ServiceClass)
            .FirstOrDefaultAsync(p => p.Id == productId);

        if (product is null)
            return TypedResults.NotFound();

        var purchase = new Purchase(
            Guid.NewGuid(),
            DateTime.UtcNow,
            user,
            product.ActualPrice.Price);

        var purchasedTicketsCount = await dbContext.PurchasedTickets
            .Where(p => p.Product.Id == product.Id)
            .CountAsync();

        if (product.ServiceClass.PlacesCount < purchasedTicketsCount + quantity)
            return TypedResults.BadRequest(NotEnoughTickets().ToDictionaryLower()); // TODO: not enough tickets

        if (product.ActualPrice.Price * quantity > user.Balance)
            return TypedResults.BadRequest(NotEnoughBalance().ToDictionaryLower()); // TODO: no balance
            

        var purchasedTickets = new List<PurchasedTicket>();
        foreach (var i in Enumerable.Range(1, quantity))
        {
            var purchasedTicket = new PurchasedTicket(
                Guid.NewGuid(),
                product,
                purchase,
                product.ActualPrice,
                seatNumber: (short)(purchasedTicketsCount + i),
                ticketIdentifier:
                $"{product.Flight.FlightNumber}{product.ServiceClass.RowsOffset}{product.ServiceClass.Title[0]}{(short)(purchasedTicketsCount + 1):0000}",
                quantity: 1
            );

            user.Balance -= product.ActualPrice.Price;
            await dbContext.PurchasedProducts.AddAsync(purchasedTicket);
            purchasedTickets.Add(purchasedTicket);
        }

        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(purchasedTickets.Select(PurchasedTicketDto.Map));
    }
    
    private static ValidationResult NotEnoughBalance()
        => GenerateValidationResult(
            new ValidationFailure("errors", "Не хватает средств, чтобы оформить покупку!"));
    
    private static ValidationResult NotEnoughTickets()
        => GenerateValidationResult(
            new ValidationFailure("errors", "Невозможно приобрести запрошенное количество товара!"));

    private static async Task<Results<Ok<ProductDto>, NotFound, BadRequest<IDictionary<string, string[]>>>> ChangePrice(
        AirportDbContext dbContext,
        ClaimsPrincipal userClaims,
        Guid productId,
        double newPrice)
    {
        var user = await GetUserByClaims(dbContext, userClaims);

        var product = await dbContext.Products.FirstOrDefaultAsync(p => p.Id == productId);
        if (product is null)
            return TypedResults.NotFound();

        if (newPrice <= 0)
            return TypedResults.BadRequest(PriceLessThenZeroValidationError().ToDictionaryLower());

        var price = new PriceChange(
            Guid.NewGuid(),
            DateTime.UtcNow,
            user,
            product,
            newPrice
        );

        await dbContext.PriceChanges.AddAsync(price);
        await dbContext.SaveChangesAsync();

        return TypedResults.Ok(ProductDto.Map(product));
    }

    private static ValidationResult PriceLessThenZeroValidationError()
        => GenerateValidationResult(
            new ValidationFailure("errors", "Цена должна быть больше 0!"));

    private static ValidationResult GenerateValidationResult(params ValidationFailure[] failures) => new(failures);

    private static async Task<Ok<IEnumerable<TicketProductDto>>> GetProducts(AirportDbContext dbContext, int pageNumber = 1, int pageSize = 10)
    {
        var products = await dbContext.Products
            .OfType<TicketProduct>()
            .Where(p => !p.Deleted)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToArrayAsync();

        return TypedResults.Ok(products.Select(p => TicketProductDto.Map(p, dbContext)));
    }

    private static async Task<Results<Ok, NotFound>> DeleteProduct(AirportDbContext dbContext, Guid productId)
    {
        var deletedProducts = await dbContext.Products
            .Where(p => p.Id == productId)
            .ExecuteUpdateAsync(p => p.SetProperty(x => x.Deleted, _ => true));

        return deletedProducts > 0 ? TypedResults.Ok() : TypedResults.NotFound();
    }

    private static async Task<Results<Ok<IEnumerable<RowClassDto>>, NotFound>> GetRowClassesByFlightId(
        AirportDbContext dbContext, Guid flightId, bool includeProducts = true)
    {
        var flight = await dbContext.Flights
            .Include(f => f.Airplane)
            .ThenInclude(a => a.Model)
            .ThenInclude(a => a.RowClasses)
            .FirstOrDefaultAsync(f => f.Id == flightId);

        if (flight == null)
        {
            return TypedResults.NotFound();
        }

        var rowClassesDtos = flight.Airplane
            .Model
            .RowClasses
            .Where(rc =>
                includeProducts || !dbContext.Products.OfType<TicketProduct>()
                    .Any(p => p.FlightId == flightId && p.ServiceClass.Id == rc.Id))
            .Select(RowClassDto.Map).ToList();
        return TypedResults.Ok(rowClassesDtos.AsEnumerable());
    }

    private static async Task<Results<Ok, ValidationProblem>> CreateFlightProduct(
        AirportDbContext dbContext,
        IValidator<CreateTicketProductRequest> validator,
        ClaimsPrincipal userClaims,
        [FromBody] CreateTicketProductRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return TypedResults.ValidationProblem(validationResult.ToDictionaryLower());
        }

        var user = await GetUserByClaims(dbContext, userClaims);

        var flight = await dbContext.Flights
            .Include(flight => flight.Airplane)
            .ThenInclude(airplane => airplane.Model)
            .ThenInclude(model => model.RowClasses)
            .Include(flight => flight.DepartureAirport)
            .Include(flight => flight.ArrivalAirport)
            .FirstAsync(f => f.Id == request.FlightId);

        foreach (var rowClass in flight.Airplane.Model.RowClasses)
        {
            var rowClassProduct = request.RowClassProducts.FirstOrDefault(r => r.RowClassId == rowClass.Id);
            if (rowClassProduct is null) continue;

            var product = new TicketProduct(
                Guid.NewGuid(),
                $"[{flight.FlightNumber}] – {rowClass.Title} – {flight.DepartureAirport.IataCode} -> {flight.ArrivalAirport.IataCode}",
                user,
                flight,
                rowClass,
                rowClass.RowsCount * rowClass.SeatsPerRow
            );

            var price = new PriceChange(
                Guid.NewGuid(),
                DateTime.UtcNow,
                user,
                product,
                rowClassProduct.Price
            );

            await dbContext.Products.AddAsync(product);
            await dbContext.PriceChanges.AddAsync(price);
        }

        await dbContext.SaveChangesAsync();

        return TypedResults.Ok();
    }

    private static Task<User> GetUserByClaims(AirportDbContext dbContext, ClaimsPrincipal userClaims)
    {
        var userIdClaim = userClaims.Identities
            .SelectMany(i => i.Claims)
            .FirstOrDefault(c => c.Type == AuthClaims.UserId);

        Guid.TryParse(userIdClaim!.Value, out var userId);
        return dbContext
            .Users
            .FirstAsync(u => u.Id == userId);
    }
}

public record ProductDto(
    Guid Id,
    string Title,
    double Price,
    bool Deleted,
    long? Quantity,
    string Type)
{
    public static ProductDto Map(Product product)
        => new ProductDto(
            product.Id,
            product.Title,
            product.ActualPrice.Price,
            product.Deleted,
            product.Quantity,
            product.Discriminator
        );
}

public record TicketProductDto(
    Guid Id,
    string Title,
    double Price,
    bool Deleted,
    long? Quantity,
    string Type,
    TableFlightDto Flight,
    RowClassDto ServiceClass,
    int BoughtTicketsCount)
    : ProductDto(Id, Title, Price, Deleted, Quantity, Type)
{
    public static TicketProductDto Map(TicketProduct product, AirportDbContext dbContext)
        => new(
            product.Id,
            product.Title,
            product.ActualPrice.Price,
            product.Deleted,
            product.Quantity,
            product.Discriminator,
            TableFlightDto.Map(product.Flight),
            RowClassDto.Map(product.ServiceClass),
            dbContext.PurchasedProducts.OfType<PurchasedTicket>().Count(t => t.Product.Id == product.Id)
        );
}

public record PurchasedTicketDto(
    Guid Id,
    string Title,
    double Price,
    int SeatNumber,
    string TicketNumber,
    TableFlightDto Flight,
    RowClassDto ServiceClass,
    Guid PurchaseId
)
{
    public static PurchasedTicketDto Map(PurchasedTicket ticket) =>
        new(
            ticket.Id,
            ticket.Product.Title,
            ticket.ActualPrice.Price,
            ticket.SeatNumber,
            ticket.TicketNumber,
            TableFlightDto.Map(ticket.Product.Flight),
            RowClassDto.Map(ticket.Product.ServiceClass),
            ticket.Purchase.Id);
}

public record CreateTicketProductRequest(
    Guid FlightId,
    IEnumerable<RowClassProductDto> RowClassProducts);

public record RowClassProductDto(
    int RowClassId,
    double Price
);

public record RowClassDto(
    int Id,
    string Title,
    int RowsCount,
    int RowsOffset,
    int SeatsPerRow,
    short ServiceLevel
)
{
    public static RowClassDto Map(RowClass rowClass)
    {
        return new RowClassDto(
            rowClass.Id,
            rowClass.Title,
            rowClass.RowsCount,
            rowClass.RowsOffset,
            rowClass.SeatsPerRow,
            rowClass.ServiceLevel);
    }
};

public class CreateTicketProductRequestValidator : AbstractValidator<CreateTicketProductRequest>
{
    public CreateTicketProductRequestValidator(AirportDbContext dbContext)
    {
        RuleFor(r => r.FlightId)
            .MustAsync(async (id, ct) => await dbContext.Flights.AnyAsync(f => id == f.Id, ct))
            .WithMessage("Рейс не найден.");

        RuleForEach(r => r.RowClassProducts)
            .MustAsync(async (p, rcp, ct) =>
            {
                var flight = await dbContext.Flights
                    .Include(flight => flight.Airplane)
                    .ThenInclude(airplane => airplane.Model)
                    .ThenInclude(airplaneModel => airplaneModel.RowClasses)
                    .FirstAsync(f => f.Id == p.FlightId, cancellationToken: ct);

                var rowClass =
                    await dbContext.RowClasses.FirstOrDefaultAsync(rc => rc.Id == rcp.RowClassId,
                        cancellationToken: ct);
                return rowClass != null && flight.Airplane.Model.RowClasses.Contains(rowClass);
            })
            .WithMessage("Класс обслуживания для рейса не найден.")
            .Must(rcp => rcp.Price > 0)
            .WithMessage("Цена должна быть больше ноля!");
    }
}

// TODO:
// Products search with pagination
// EDIT PRICE ON PRODUCT +
// DELETE PRODUCTS +
// BUY PRODUCTS +
// Get bought products +
// LOAD USERS WITH JSON BY API +
// FRONT!!!!
// FIX RIGHTS