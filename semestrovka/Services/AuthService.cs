using semestrovka.DAOs;
using semestrovka.Models;

namespace semestrovka.Services;

public class AuthService
{
    public string GenerateToken(User user)
    {
        return user.Email + user.Password;
    }
    
    public bool ValidateToken(string token)
    {
        var userDao = new UserDao();
        var users = userDao.GetUsers();
        return users.Select(user =>
                (user.Email + user.Password))
            .Any(userToken => token == userToken);
    }
}