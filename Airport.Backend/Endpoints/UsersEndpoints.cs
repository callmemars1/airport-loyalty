using System.Security.Claims;
using System.Text.Json;
using Airport.Backend.Utils;
using Airport.Model;
using Airport.Model.Users;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Airport.Backend.Endpoints;

public static class UsersEndpoints
{
    public static void RegisterUserEndpoints(this IEndpointRouteBuilder app)
    {
        var usersEndpointsGroup = app.MapGroup("/users");

        usersEndpointsGroup.MapGet("/", GetUsersAsync).RequireAuthorization(p =>
        {
            p.RequireClaim(AuthClaims.Role, Roles.Admin.ToString(), Roles.Editor.ToString());
        });
        usersEndpointsGroup.MapGet("/current-data", GetCurrentUserDataAsync).RequireAuthorization();
        
        usersEndpointsGroup.MapGet("/roles", GetRoles).RequireAuthorization(p =>
        {
            p.RequireClaim(AuthClaims.Role, Roles.Admin.ToString(), Roles.Editor.ToString());
        });
        
        usersEndpointsGroup.MapGet("/by-id", GetUserDataById).RequireAuthorization(p =>
        {
            p.RequireClaim(AuthClaims.Role, Roles.Admin.ToString(), Roles.Editor.ToString());
        });
        
        usersEndpointsGroup.MapDelete("/self-delete", SelfDelete).RequireAuthorization();
        usersEndpointsGroup.MapDelete("/delete", DeleteUser).RequireAuthorization(p =>
        {
            p.RequireClaim(AuthClaims.Role, Roles.Admin.ToString(), Roles.Editor.ToString());
        });
        
        usersEndpointsGroup.MapPut("/update", UpdateUser).RequireAuthorization();

        usersEndpointsGroup.MapPatch("/add-balance", AddBalance).RequireAuthorization();
        
        usersEndpointsGroup.MapPost("/upload", UploadUsers).RequireAuthorization(p =>
        {
            p.RequireClaim(AuthClaims.Role, Roles.Admin.ToString(), Roles.Editor.ToString());
        }).DisableAntiforgery();
    }

    public static async Task<Ok> AddBalance(AirportDbContext dbContext, ClaimsPrincipal userClaims, double sumToAdd)
    {
        var user = await GetUserByClaims(dbContext, userClaims);
        user.Balance += sumToAdd;
        await dbContext.SaveChangesAsync();
        return TypedResults.Ok();
    }
    
    private static async Task<Results<Created, ValidationProblem>> UploadUsers(
        IFormFile file,
        IValidator<CreateUserRequest> validator,
        IPasswordHasher passwordHasher,
        AirportDbContext dbContext)
    {
        if (file == null || file.Length == 0)
            return TypedResults.ValidationProblem(CreateValidationError("Can't read JSON!").ToDictionaryLower());
        
        if (Path.GetExtension(file.FileName) != ".json")
            return TypedResults.ValidationProblem(CreateValidationError("Only JSON are supported!").ToDictionaryLower());

        List<CreateStaffRequest> users;
        try
        {
            using var reader = new StreamReader(file.OpenReadStream());
            var content = await reader.ReadToEndAsync();
            users = JsonSerializer.Deserialize<List<CreateStaffRequest>>(content)!;
        }
        catch (JsonException)
        {
            return TypedResults.ValidationProblem(CreateValidationError("Can't read JSON!").ToDictionaryLower());
        }

        foreach (var createUserRequest in users!)
        {
            var validationResult = await validator.ValidateAsync(createUserRequest);
            if (!validationResult.IsValid)
                return TypedResults.ValidationProblem(validationResult.ToDictionaryLower());
            
            Enum.TryParse(createUserRequest.Role, out Roles parsedRole);
            var passwordData = passwordHasher.HashPassword(createUserRequest.Password);
            var user = new User(
                id: Guid.NewGuid(),
                name: createUserRequest.Name,
                surname: createUserRequest.Surname,
                patronymic: string.IsNullOrEmpty(createUserRequest.Patronymic) ? null : createUserRequest.Patronymic,
                passportNumber: createUserRequest.PassportNumber,
                createdAt: DateTime.UtcNow,
                login: createUserRequest.Login,
                passwordHash: passwordData.PasswordHash,
                salt: passwordData.Salt,
                role: await dbContext.Roles.SingleAsync(r => r.SystemName == parsedRole)
            );
            await dbContext.Users.AddAsync(user);
        }
        await dbContext.SaveChangesAsync();

        return TypedResults.Created();
    }

    private static ValidationResult CreateValidationError(string error)
        => GenerateValidationResult(
            new ValidationFailure("errors", error));

    private static ValidationResult GenerateValidationResult(params ValidationFailure[] failures) => new(failures);
    
    private static async Task<Ok<IEnumerable<UserDto>>> GetUsersAsync(
        AirportDbContext dbContext,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var users = await dbContext.Users
            .OrderBy(u => u.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return TypedResults.Ok(users.Select(UserDto.Map));
    }

    private static async Task<Results<Ok<UserDto>, UnauthorizedHttpResult, NotFound, ValidationProblem>> UpdateUser(
        [FromBody] UpdateUserRequest request,
        IValidator<UpdateUserRequest> validator,
        IPasswordHasher passwordHasher,
        AirportDbContext dbContext,
        ClaimsPrincipal userClaims)
    {
        var changer = await GetUserByClaims(dbContext, userClaims);
        
        var userToUpdate = await dbContext
            .Users
            .Include(user => user.Role)
            .FirstOrDefaultAsync(u => u.Id == request.Id);

        if (userToUpdate is null)
            return TypedResults.NotFound();

        var r = await validator.ValidateAsync(request);

        if (!r.IsValid)
            return TypedResults.ValidationProblem(r.ToDictionaryLower());

        

        Roles? requestedRole;
        if (request.Role is not null)
        {
            Enum.TryParse(request.Role, out Roles parsedRequestedRole);
            if (changer.Role.SystemName is Roles.Client && changer.Id != request.Id)
                return TypedResults.Unauthorized();

            if (changer.Role.SystemName is not Roles.Admin && parsedRequestedRole != userToUpdate.Role.SystemName)
                return TypedResults.Unauthorized();

            requestedRole = parsedRequestedRole;
        }
        else requestedRole = userToUpdate.Role.SystemName;

        string hash;
        string salt;
        if (request.Password is null or "")
        {
            hash = userToUpdate.PasswordHash;
            salt = userToUpdate.Salt;
        }
        else if (passwordHasher.CheckPassword(
                     request.Password,
                     new HashedPasswordData(userToUpdate.PasswordHash, userToUpdate.Salt)))
        {
            hash = userToUpdate.PasswordHash;
            salt = userToUpdate.Salt;
        }
        else
        {
            var hashedData = passwordHasher.HashPassword(request.Password);
            hash = hashedData.PasswordHash;
            salt = hashedData.Salt;
        }
            

        userToUpdate.UpdateData(
            name: request.Name,
            surname: request.Surname,
            patronymic: request.Patronymic,
            passportNumber: request.PassportNumber,
            role: await dbContext.Roles.FirstAsync(r => r.SystemName == requestedRole),
            passwordHash: hash,
            salt: salt);

        await dbContext.SaveChangesAsync();
        return TypedResults.Ok(UserDto.Map(userToUpdate));
    }

    private static async Task<Ok<UserDto>> GetCurrentUserDataAsync(AirportDbContext dbContext,
        ClaimsPrincipal userClaims)
    {
        var user = await GetUserByClaims(dbContext, userClaims);

        return TypedResults.Ok(UserDto.Map(user));
    }
    
    private static async Task<Results<Ok<UserDto>, NotFound>> GetUserDataById(
        AirportDbContext dbContext,
        Guid userId)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == userId);
        if (user is null)
            return TypedResults.NotFound();

        return TypedResults.Ok(UserDto.Map(user));
    }

    private static async Task<SignOutHttpResult> SelfDelete(AirportDbContext dbContext, ClaimsPrincipal userClaims)
    {
        var user = await GetUserByClaims(dbContext, userClaims);

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync();

        return TypedResults.SignOut();
    }

    private static async Task<Ok> DeleteUser(AirportDbContext dbContext, Guid userId)
    {
        await dbContext
            .Users
            .Where(u => u.Id == userId)
            .ExecuteDeleteAsync();

        return TypedResults.Ok();
    }

    private static async Task<Ok<IEnumerable<RoleDto>>> GetRoles(AirportDbContext dbContext) => TypedResults.Ok(
        (await dbContext.Roles.ToArrayAsync()).Select(RoleDto.ToDto)
    );

    private static async Task<User> GetUserByClaims(AirportDbContext dbContext, ClaimsPrincipal userClaims)
    {
        var userIdClaim = userClaims.Identities
            .SelectMany(i => i.Claims)
            .FirstOrDefault(c => c.Type == AuthClaims.UserId);

        Guid.TryParse(userIdClaim!.Value, out var userId);
        return await dbContext
            .Users
            .FirstAsync(u => u.Id == userId);
    }
}

public record UserDto(
    Guid Id,
    string Name,
    string Surname,
    string? Patronymic,
    string PassportNumber,
    DateTime CreatedAt,
    string Login,
    string Role,
    double Balance
)
{
    public static UserDto Map(User user) => new(
        user.Id,
        user.Name,
        user.Surname,
        user.Patronymic,
        user.PassportNumber,
        user.CreatedAt,
        user.Login,
        user.Role.SystemName.ToString(),
        user.Balance
    );
}

public record UpdateUserRequest(
    Guid Id,
    string Name,
    string Surname,
    string? Patronymic,
    string PassportNumber,
    string? Role,
    string? Password)
{
}



public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator(IPasswordHasher passwordHasher, AirportDbContext dbContext)
    {
        RuleFor(r => r.Name)
            .NotNull()
            .MinimumLength(2).WithMessage("Имя не должно быть короче 2 символов.")
            .MaximumLength(128).WithMessage("Имя не должно быть длинее 128 символов.")
            .Matches("^[a-zA-Zа-яА-Я]+$").WithMessage(
                "Поле должно содержать только буквы, без цифр и специальных символов, допустимы латинские и кириллические буквы.");

        RuleFor(r => r.Surname)
            .NotNull()
            .MinimumLength(2).WithMessage("Фамилия не должна быть короче 2 символов.")
            .MaximumLength(512).WithMessage("Фамилия не должна быть длинее 128 символов.")
            .Matches("^[a-zA-Zа-яА-Я]+$").WithMessage(
                "Поле должно содержать только буквы, без цифр и специальных символов, допустимы латинские и кириллические буквы.");

        RuleFor(r => r.Patronymic)
            .MinimumLength(2).When(p => p is not null).WithMessage("Отчество не должно быть короче 2 символов.")
            .MaximumLength(512).When(p => p is not null).WithMessage("Отчество не должно быть длинее 128 символов.")
            .Matches("^[a-zA-Zа-яА-Я]+$").When(p => p is not null).WithMessage(
                "Поле должно содержать только буквы, без цифр и специальных символов, допустимы латинские и кириллические буквы.");
        
        RuleFor(r => r.PassportNumber)
            .NotNull()
            .Matches(@"^\d{4}\s\d{6}$").WithMessage(
                "Должно быть ровно 10 цифр, разделённые пробелом после четвёртой цифры.");
    }
}