using System.Security.Claims;
using Airport.Backend.Utils;
using Airport.Model;
using Airport.Model.Users;
using FluentValidation;
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
    }

    public static async Task<Results<Created, ValidationProblem>> SignUp(
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
            patronymic: request.Patronymic,
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

    public static async Task<Results<SignInHttpResult, NotFound>> SignIn(
        AirportDbContext dbContext,
        IPasswordHasher passwordHasher,
        [FromBody] SignInRequest request)
    {
        var user = await dbContext.Users.SingleOrDefaultAsync(u => u.Login == request.Login);

        if (user is null) 
            return TypedResults.NotFound();

        if (!passwordHasher.CheckPassword(request.Password, new HashedPasswordData(user.PasswordHash, user.Salt)))
            return TypedResults.NotFound();

        var claims = new List<Claim>
        {
            new ("UserId", user.Id.ToString())
        };
        var claimsIdentity = new ClaimsIdentity(claims, "Cookies");
        return TypedResults.SignIn(new ClaimsPrincipal(claimsIdentity));
    }
}

public record SignUpRequest(
    string Name,
    string Surname,
    string? Patronymic,
    string PassportNumber,
    string Login,
    string Password
);

public class SignUpRequestValidator : AbstractValidator<SignUpRequest>
{
    private readonly AirportDbContext _dbContext;

    public SignUpRequestValidator(AirportDbContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(r => r.Login)
            .NotNull()
            .MinimumLength(2)
            .MaximumLength(128)
            .Matches("^[a-zA-Z0-9_-]+$")
            .WithMessage(
                "Логин должен содержать только латинские буквы, цифры, символы подчёркивания или дефиса, без пробелов.");

        RuleFor(r => r.Login)
            .NotNull()
            .MustAsync(LoginNotExist)
            .WithMessage("Логин занят.");

        RuleFor(r => r.Password)
            .NotNull()
            .MaximumLength(128)
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$")
            .WithMessage("Пароль должен содержать минимум 8 символов, должен включать строчные и заглавные буквы, а также цифры.");

        RuleFor(r => r.Name)
            .NotNull()
            .MinimumLength(2)
            .MaximumLength(512)
            .Matches("^[a-zA-Zа-яА-Я]+$")
            .WithMessage(
                "Поле должно содержать только буквы, без цифр и специальных символов, допустимы латинские и кириллические буквы.");
        
        RuleFor(r => r.Surname)
            .NotNull()
            .MinimumLength(2)
            .MaximumLength(512)
            .Matches("^[a-zA-Zа-яА-Я]+$")
            .WithMessage(
                "Поле должно содержать только буквы, без цифр и специальных символов, допустимы латинские и кириллические буквы.");

        RuleFor(r => r.Patronymic)
            .MinimumLength(2).When(p => p is not null)
            .MaximumLength(512).When(p => p is not null)
            .Matches("^[a-zA-Zа-яА-Я]+$").When(p => p is not null)
            .WithMessage(
                "Поле должно содержать только буквы, без цифр и специальных символов, допустимы латинские и кириллические буквы.");

        RuleFor(r => r.PassportNumber)
            .NotNull()
            .Matches(@"^\d{4}\s\d{6}$")
            .WithMessage("Должно быть ровно 10 цифр, разделённые пробелом после четвёртой цифры.");

    }

    private async Task<bool> LoginNotExist(string login, CancellationToken cancellationToken) =>
        await _dbContext.Users.AllAsync(u => u.Login != login, cancellationToken: cancellationToken);

}



public record SignInRequest(string Login, string Password);

public record UserCredentialsDto(string Login, string Password);