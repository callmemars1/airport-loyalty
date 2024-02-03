namespace Airport.Model.Users;

public class User
{
    // EF
    protected User()
    {
    }

    public User(
        Guid id,
        string name,
        string surname,
        string? patronymic,
        string passportNumber,
        DateTime createdAt,
        string login,
        string passwordHash,
        string salt,
        Role role,
        double balance = 0)
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
        Role = role;
        Balance = balance;
    }

    public double Balance { get; set; }

    public Guid Id { get; private set; }

    public string Name { get; private set; } = null!;

    public string Surname { get; private set; } = null!;

    public string? Patronymic { get; private set; } = null!;

    public string PassportNumber { get; private set; } = null!;

    public DateTime CreatedAt { get; private set; }

    public string Login { get; private set; }

    public string PasswordHash { get; private set; }
    public string Salt { get; private set; }

    public virtual Role Role { get; private set; }

    public void UpdateData(
        string name,
        string surname,
        string? patronymic,
        string passportNumber,
        Role role,
        string passwordHash,
        string salt)
    {
        Name = name;
        Surname = surname;
        Patronymic = patronymic;
        PassportNumber = passportNumber;
        Role = role;
        PasswordHash = passwordHash;
        Salt = salt;
    }
}