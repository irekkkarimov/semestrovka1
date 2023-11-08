using System.Net.Http.Json;
using semestrovka.Attributes;
using semestrovka.DAOs;
using semestrovka.DTOs;
using semestrovka.Mapper;
using semestrovka.utils;

namespace semestrovka.Controllers;

[Controller("User")]
public class UserController
{
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
            return user.Password == userFromDb.Password ? 
                new ResponseMessage(200, "Authorized") 
                : new ResponseMessage(404, "Wrong password");

        return new ResponseMessage(404, "Wrong email");
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