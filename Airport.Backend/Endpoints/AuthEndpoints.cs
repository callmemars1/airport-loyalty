using System.Security.Claims;
using Airport.Backend.Utils;
using Airport.Model;
using Airport.Model.Users;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace Airport.Backend.Endpoints;

public static class AuthEndpoints
{
    public static void RegisterAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var authEndpointsGroup = app.MapGroup("/auth");
        authEndpointsGroup.MapPost("/sign-in", SignIn);
        authEndpointsGroup.MapPost("/sign-up", SignUp);
        authEndpointsGroup.MapPost("/create-staff", CreateStaff).RequireAuthorization(p =>
        {
            p.RequireClaim(AuthClaims.Role, Roles.Admin.ToString());
        });
        authEndpointsGroup.MapPost("/sign-out", SignOut);
        authEndpointsGroup.MapGet("/get-permissions", GetPermissions).RequireAuthorization();
    }

    private static async Task<Results<Created, ValidationProblem>> SignUp(
        AirportDbContext dbContext,
        IPasswordHasher passwordHasher,
        IValidator<CreateUserRequest> validator,
        [FromBody] SignUpRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionaryLower());

        await CreateUser(dbContext, passwordHasher, request, Roles.Client);

        return TypedResults.Created();
    }

    private static async Task<Results<Created, ValidationProblem>> CreateStaff(
        AirportDbContext dbContext,
        IPasswordHasher passwordHasher,
        IValidator<CreateUserRequest> validator,
        [FromBody] CreateStaffRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionaryLower());

        Enum.TryParse(request.Role, out Roles role);
        await CreateUser(dbContext, passwordHasher, request, role);

        return TypedResults.Created();
    }

    private static async Task<Results<SignInHttpResult, NotFound<IDictionary<string, string[]>>>> SignIn(
        AirportDbContext dbContext,
        IPasswordHasher passwordHasher,
        [FromBody] SignInRequest request)
    {
        var user = await dbContext.Users
            .Include(user => user.Role)
            .SingleOrDefaultAsync(u => u.Login == request.Login);

        if (user is null)
            return TypedResults.NotFound(UserWithCredentialsNotFoundValidationError().ToDictionaryLower());

        if (!passwordHasher.CheckPassword(request.Password, new HashedPasswordData(user.PasswordHash, user.Salt)))
            return TypedResults.NotFound(UserWithCredentialsNotFoundValidationError().ToDictionaryLower());

        var claims = new List<Claim>
        {
            new(AuthClaims.UserId, user.Id.ToString()),
            new(AuthClaims.Role, user.Role.SystemName.ToString()),
        };
        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        return TypedResults.SignIn(new ClaimsPrincipal(claimsIdentity));
    }

    private static async Task<SignOutHttpResult> SignOut() => await Task.FromResult(TypedResults.SignOut());

    private static async Task<Ok<RoleDto>> GetPermissions(
        AirportDbContext dbContext,
        ClaimsPrincipal userClaims)
    {
        var userIdClaim = userClaims.Identities
            .SelectMany(i => i.Claims)
            .FirstOrDefault(c => c.Type == AuthClaims.UserId);

        Guid.TryParse(userIdClaim!.Value, out var userId);
        var user = await dbContext
            .Users
            .Include(user => user.Role)
            .FirstAsync(u => u.Id == userId);

        return TypedResults.Ok(new RoleDto(user.Role.SystemName.ToString(), user.Role.Title, user.Role.Description));
    }

    private static async Task CreateUser(
        AirportDbContext dbContext,
        IPasswordHasher passwordHasher,
        CreateUserRequest request,
        Roles role)
    {
        var passwordData = passwordHasher.HashPassword(request.Password);
        var user = new User(
            id: Guid.NewGuid(),
            name: request.Name,
            surname: request.Surname,
            patronymic: string.IsNullOrEmpty(request.Patronymic) ? null : request.Patronymic,
            passportNumber: request.PassportNumber,
            createdAt: DateTime.UtcNow,
            login: request.Login,
            passwordHash: passwordData.PasswordHash,
            salt: passwordData.Salt,
            role: await dbContext.Roles.SingleAsync(r => r.SystemName == role)
        );
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();
    }


    private static ValidationResult UserWithCredentialsNotFoundValidationError()
        => GenerateValidationResult(
            new ValidationFailure("errors", "Пользователь с данным логином и паролем не найден. Попробуйте еще раз."));

    private static ValidationResult GenerateValidationResult(params ValidationFailure[] failures) => new(failures);
}

public static class AuthClaims
{
    public const string Role = "Role";
    public const string UserId = "UserId";
}