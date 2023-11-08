using semestrovka.DTOs;
using semestrovka.Models;

namespace semestrovka.Mapper;

public class UserMapper
{
    public static UserDto MapUserToUserDto(User user)
    {
        return new UserDto
        {
            Email = user.Email,
            Password = user.Password
        };
    }

    public static User MapUserDtoToUser(UserDto userDto)
    {
        return new User
        {
            Id = 0,
            Email = userDto.Email,
            Password = userDto.Password
        };
    }
}