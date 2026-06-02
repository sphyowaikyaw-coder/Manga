using Dependency;

namespace DAO.DAO;

public interface UserDAO
{
    Task<List<User>> GetAllUsers();

    Task<User?> GetUserById(int id);

    Task<User?> GetUserByEmail(string email);

    Task<bool> CreateUser(User user);

    Task<bool> UpdateUser(User user);
}
