using semestrovka.DAOs;
using semestrovka.Models;

namespace semestrovka.Services;

public interface IAuthService
{
    string GenerateToken(User user);
    bool ValidateToken(IUserDao userDao, string token);
    (bool, int) ValidateTokenAndGetUserId(IUserDao userDao, string token);
}