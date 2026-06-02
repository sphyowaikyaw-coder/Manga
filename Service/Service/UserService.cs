using Service.Business_Model;

namespace Service.Service;

public interface UserService
{
    Task<List<BM_UserItem>> GetAllUsers();

    Task<BM_UserItem?> GetUserById(int id);

    Task<BM_UserItem?> Login(string email, string password);

    Task<bool> Register(BM_UserItem user);

    Task<bool> UpdateUser(BM_UserItem user);
}
