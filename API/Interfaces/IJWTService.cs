using API.Models;

namespace API.Interfaces;

public interface IJWTService
{
    string CreateToken(AppUser user);
}