namespace Airport.Model.Users;

public class Role
{
    public int Id { get; private set; }
    public Roles SystemName { get; private set; }
    public string Title { get; private set; }
    public string Description { get; private set; }

    // EF
    protected Role()
    {
    }

    public Role(Roles role, string title, string description)
    {
        SystemName = role;
        Title = title;
        Description = description;
    }
}

public enum Roles
{
    Client,
    Editor,
    Analyst,
    Admin
}