namespace Airport.Model.Users;

public class User
{
    public User(
        Guid id,
        string name,
        string surname,
        string? patronymic,
        string passportNumber,
        DateTime createdAt,
        string login,
        string passwordHash,
        string salt)
    {
        Id = id;
        Name = name;
        Surname = surname;
        Patronymic = patronymic;
        PassportNumber = passportNumber;
        CreatedAt = createdAt;
        Login = login;
        PasswordHash = passwordHash;
        Salt = salt;
    }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;

    public string Surname { get; private set; } = null!;

    public string? Patronymic { get; private set; } = null!;

    public string PassportNumber { get; private set; } = null!;
    
    public DateTime CreatedAt { get; private set; }
    
    public string Login { get; private set; }
    
    public string PasswordHash { get; private set; }
    public string Salt { get; private set; }
}