using Airport.Model;
using Airport.Model.Users;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Airport.Backend.Endpoints;

public record CreateUserRequest(
    string Name,
    string Surname,
    string? Patronymic,
    string PassportNumber,
    string Login,
    string Password
);

public record SignUpRequest(string Name, string Surname, string? Patronymic, string PassportNumber, string Login, string Password) 
    : CreateUserRequest(Name, Surname, Patronymic, PassportNumber, Login, Password);

public record CreateStaffRequest(
        string Name,
        string Surname,
        string? Patronymic,
        string PassportNumber,
        string Login,
        string Password,
        string Role)
    : CreateUserRequest(Name, Surname, Patronymic, PassportNumber, Login, Password)
{
};

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    private readonly AirportDbContext _dbContext;

    public CreateUserRequestValidator(AirportDbContext dbContext, IValidator<CreateStaffRequest> staffValidator)
    {
        _dbContext = dbContext;

        RuleFor(r => r.Login)
            .NotNull()
            .MinimumLength(2).WithMessage("Логин не должен быть короче 2 символов.")
            .MaximumLength(128).WithMessage("Логин не должен быть длинее 128 символов.")
            .Matches("^[a-zA-Z0-9_-]+$").WithMessage(
                "Логин должен содержать только латинские буквы, цифры, символы подчёркивания или дефиса, без пробелов.")
            .MustAsync(LoginNotExist).WithMessage("Логин занят");

        RuleFor(r => r.Password)
            .NotNull()
            .MaximumLength(128).WithMessage("Пароль должен быть короче 128 символов.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)[a-zA-Z\d]{8,}$").WithMessage(
                "Пароль должен содержать минимум 8 символов, должен включать строчные и заглавные буквы, а также цифры.");

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
            .MinimumLength(2).When(p => p.Patronymic is not null or "").WithMessage("Отчество не должно быть короче 2 символов.")
            .MaximumLength(512).When(p =>  p.Patronymic is not null or "").WithMessage("Отчество не должно быть длинее 128 символов.")
            .Matches("^[a-zA-Zа-яА-Я]+$").When(p => p.Patronymic is not null or "").WithMessage(
                "Поле должно содержать только буквы, без цифр и специальных символов, допустимы латинские и кириллические буквы.");

        RuleFor(r => r.PassportNumber)
            .NotNull()
            .Matches(@"^\d{4}\s\d{6}$").WithMessage(
                "Должно быть ровно 10 цифр, разделённые пробелом после четвёртой цифры.");
        
        RuleFor(r => r).SetInheritanceValidator(v => v.Add(staffValidator));
    }

    private async Task<bool> LoginNotExist(string login, CancellationToken cancellationToken) =>
        await _dbContext.Users.AllAsync(u => u.Login != login, cancellationToken: cancellationToken);
}

public class CreateStaffRequestValidator : AbstractValidator<CreateStaffRequest>
{
    public CreateStaffRequestValidator(AirportDbContext dbContext)
    {
        RuleFor(r => r.Role)
            .NotNull().WithMessage("""Поле "Роль" не заполнено!""")
            .Must(r => Enum.TryParse(r, out Roles _))
            .WithMessage("Роль не найдена!");
    }
}
