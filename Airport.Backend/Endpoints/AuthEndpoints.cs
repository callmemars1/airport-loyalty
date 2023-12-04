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
        authEndpointsGroup.MapPost("/sign-in", SignIn).AllowAnonymous();
        authEndpointsGroup.MapPost("/sign-up", SignUp).AllowAnonymous();
        authEndpointsGroup.MapGet("/check-authorized", CheckAuthorized).RequireAuthorization();
        authEndpointsGroup.MapPost("/sign-out", SignOut).RequireAuthorization();
    }

    private static async Task<Results<Created, ValidationProblem>> SignUp(
        AirportDbContext dbContext,
        IPasswordHasher passwordHasher,
        IValidator<SignUpRequest> validator,
        [FromBody] SignUpRequest request)
    {
        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
            return TypedResults.ValidationProblem(validationResult.ToDictionaryLower());

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
            salt: passwordData.Salt
        );
        await dbContext.Users.AddAsync(user);
        await dbContext.SaveChangesAsync();

        return TypedResults.Created();
    }

    private static async Task<Results<SignInHttpResult, NotFound<IDictionary<string, string[]>>>> SignIn(
        AirportDbContext dbContext,
        IPasswordHasher passwordHasher,
        [FromBody] SignInRequest request)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Login == request.Login);

        if (user is null)
            return TypedResults.NotFound(UserWithCredentialsNotFoundValidationError().ToDictionaryLower());

        if (!passwordHasher.CheckPassword(request.Password, new HashedPasswordData(user.PasswordHash, user.Salt)))
            return TypedResults.NotFound(UserWithCredentialsNotFoundValidationError().ToDictionaryLower());

        var claims = new List<Claim>
        {
            new("UserId", user.Id.ToString())
        };
        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        return TypedResults.SignIn(new ClaimsPrincipal(claimsIdentity));
    }

    private static async Task<SignOutHttpResult> SignOut()
        => await Task.FromResult(TypedResults.SignOut());

    private static async Task<Ok> CheckAuthorized() => await Task.FromResult(TypedResults.Ok());

    private static ValidationResult UserWithCredentialsNotFoundValidationError()
        => GenerateValidationResult(
            new ValidationFailure("errors", "Пользователь с данным логином и паролем не найден. Попробуйте еще раз."));

    private static ValidationResult GenerateValidationResult(params ValidationFailure[] failures) => new(failures);
}