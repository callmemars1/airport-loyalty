using Airport.Model.Users;

namespace Airport.Backend.Endpoints;

record RoleDto(
    string SystemName,
    string Title,
    string Description)
{
    public static RoleDto ToDto(Role role) => new(role.SystemName.ToString(), role.Title, role.Description);
}