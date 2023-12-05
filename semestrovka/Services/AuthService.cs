using semestrovka.DAOs;
using semestrovka.Models;

namespace semestrovka.Services;

public class AuthService : IAuthService
{
    
    public string GenerateToken(User user)
    {
        return user.Email + user.Password;
    }
    
    public bool ValidateToken(IUserDao userDao, string token)
    {
        var users = userDao.GetUsers();
        return users.Select(user =>
                (user.Email + user.Password))
            .Any(userToken => token == userToken);
    }

    public (bool, int) ValidateTokenAndGetUserId(IUserDao userDao, string token)
    {
        var users = userDao.GetUsers();

        var userWithCurrentToken = users.FirstOrDefault(i => (i.Email + i.Password) == token);

        return userWithCurrentToken != null
            ? (true, userWithCurrentToken.Id)
            : (false, 0);
    }
}