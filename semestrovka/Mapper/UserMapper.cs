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
            Name = user.Name,
            Password = user.Password
        };
    }

    public static User MapUserDtoToUser(UserDto userDto)
    {
        return new User
        {
            Id = 0,
            Email = userDto.Email,
            Name = userDto.Name,
            Password = userDto.Password
        };
    }
}