using System.Net.Http.Json;
using semestrovka.Attributes;
using semestrovka.DAOs;
using semestrovka.DTOs;
using semestrovka.Handlers;
using semestrovka.Mapper;
using semestrovka.utils;

namespace semestrovka.Controllers;

// [Controller("User")]
public class UserController
{
    [Get("Login")]
    public static ResponseMessage Login()
    {
        var page = StaticFilesHandler.FetchStaticFile("login.html");
        return new ResponseMessage(200, page);
    }
    
    [Post("Login")]
    public static ResponseMessage Login(string email, string password)
    {
        var userToAuthorize = new UserDto
        {
            Email = email,
            Password = password
        };
        var user = UserMapper.MapUserDtoToUser(userToAuthorize);
        var userDao = new UserDao();
        var userFromDb = userDao.GetUserByEmail(user);
        if (user.Email == userFromDb.Email)
            if (user.Password == userFromDb.Password)
            {
                return new ResponseMessage(200, "setcookie token 123");
            }

        return new ResponseMessage(404, "Wrong email");
    }

    [Get("Registration")]
    public static ResponseMessage Registration()
    {
        var page = StaticFilesHandler.FetchStaticFile("registration.html");
        return new ResponseMessage(200, page);
    }
    
    [Post("Registration")]
    public static ResponseMessage Registration(string email, string password)
    {
        var userToRegister = new UserDto
        {
            Email = email,
            Password = password
        };
        var user = UserMapper.MapUserDtoToUser(userToRegister);
        var userDao = new UserDao();
        return userDao.CreateUser(user)
            ? new ResponseMessage(200, "Successfully registered")
            : new ResponseMessage(404, "Some error occured");
    }
}