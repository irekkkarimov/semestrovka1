using semestrovka.Models;

namespace semestrovka.DAOs;

public interface IUserDao
{
    public bool CreateUser(User user);
    public List<User> GetUsers();
    public User GetUserById(int id);
    public User GetUserByEmail(User user);
    public User DeleteUser(int id);
}