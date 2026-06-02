using DAO.DAO;
using Dependency;
using Service.Business_Model;

namespace Service.Service.ServiceImpl;

public class UserServiceImpl(UserDAO userDAO) : UserService
{
    public async Task<List<BM_UserItem>> GetAllUsers()
    {
        var users = await userDAO.GetAllUsers();

        return users.Select(MapToBusinessModel).ToList();
    }

    public async Task<BM_UserItem?> GetUserById(int id)
    {
        var user = await userDAO.GetUserById(id);

        return user is null ? null : MapToBusinessModel(user);
    }

    public async Task<BM_UserItem?> Login(string email, string password)
    {
        var user = await userDAO.GetUserByEmail(email);

        if (user is null || user.PasswordHash != password)
        {
            return null;
        }

        return MapToBusinessModel(user);
    }

    public async Task<bool> Register(BM_UserItem user)
    {
        var existingUser = await userDAO.GetUserByEmail(user.Email);

        if (existingUser is not null)
        {
            return false;
        }

        return await userDAO.CreateUser(new User
        {
            UserName = user.Name,
            Email = user.Email,
            PasswordHash = user.Password,
            Role = string.IsNullOrWhiteSpace(user.Role) ? "User" : user.Role,
            CreatedAt = DateTime.Now
        });
    }

    public async Task<bool> UpdateUser(BM_UserItem user)
    {
        return await userDAO.UpdateUser(new User
        {
            UserId = user.Id,
            UserName = user.Name,
            Email = user.Email,
            Role = string.IsNullOrWhiteSpace(user.Role) ? "User" : user.Role
        });
    }

    private static BM_UserItem MapToBusinessModel(User user)
    {
        return new BM_UserItem
        {
            Id = user.UserId,
            Name = user.UserName,
            Email = user.Email,
            Role = user.Role ?? "User",
            Joined = user.CreatedAt?.ToString("MMM yyyy") ?? string.Empty
        };
    }
}
