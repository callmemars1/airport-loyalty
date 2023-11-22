using System.Security.Claims;

namespace Airport.Backend.Utils;

public interface IJwtService
{
    public string GenerateToken(List<Claim> claims);
}