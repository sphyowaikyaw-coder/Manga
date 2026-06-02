using Dependency;
using Microsoft.EntityFrameworkCore;

namespace DAO.DAO.DAOImpl;

public class UserDAOImpl(MangaDbContext mangaDbContext) : UserDAO
{
    public async Task<List<User>> GetAllUsers()
    {
        return await mangaDbContext.Users
            .AsNoTracking()
            .OrderByDescending(user => user.UserId)
            .ToListAsync();
    }

    public async Task<User?> GetUserById(int id)
    {
        return await mangaDbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.UserId == id);
    }

    public async Task<User?> GetUserByEmail(string email)
    {
        return await mangaDbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task<bool> CreateUser(User user)
    {
        await mangaDbContext.Users.AddAsync(user);
        await mangaDbContext.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateUser(User user)
    {
        var existing = await mangaDbContext.Users.FindAsync(user.UserId);

        if (existing is null)
        {
            return false;
        }

        existing.UserName = user.UserName;
        existing.Email = user.Email;
        existing.Role = user.Role;
        existing.ProfileImage = user.ProfileImage;

        await mangaDbContext.SaveChangesAsync();
        return true;
    }
}
