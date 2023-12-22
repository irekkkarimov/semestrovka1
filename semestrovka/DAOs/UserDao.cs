using Microsoft.Extensions.Logging;
using semestrovka.Models;
using semestrovka.utils;

namespace semestrovka.DAOs;

public class UserDao : IUserDao
{
    public bool CreateUser(User user)
    {
        var dbContext = ServerData.Instance().DbContext;
        return dbContext.Add(user);
    }

    public List<User> GetUsers()
    {
        var dbContext = ServerData.Instance().DbContext;
        return dbContext.Select<User>();
    }
    
    public User GetUserById(int id)
    {
        var dbContext = ServerData.Instance().DbContext;
        return dbContext.SelectById<User>(id);
    }

    public User GetUserByEmail(User user)
    {
        var dbContext = ServerData.Instance().DbContext;
        var users = dbContext.Select<User>();
        var userByEmail = users.FirstOrDefault(i => i.Email == user.Email);
        
        return userByEmail ?? new User
        {
            Id = 0,
            Email = "Не найден",
            Password = "Не найден"
        };
    }

    public User DeleteUser(int id)
    {
        var dbContext = ServerData.Instance().DbContext;
        var user = dbContext.SelectById<User>(id);
        dbContext.Delete(user);
        return user;
    }
}