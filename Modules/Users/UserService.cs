using Microsoft.EntityFrameworkCore;

public class UserService
{
    private readonly AppDbContext _context;

    public UserService(AppDbContext context) => _context = context;

    public async Task<UserResponseDto> CreateUser(string username, string password)
    {
        var user = new User { Username = username, Password = PasswordHasher.Hash(password) };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return new UserResponseDto(user.Id, user.Username, new List<string>());
    }

    public async Task<List<UserResponseDto>> GetUsers()
    {
        return await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Select(u => new UserResponseDto(
                u.Id,
                u.Username,
                u.UserRoles.Select(ur => ur.Role.Name).ToList()
            ))
            .ToListAsync();
    }

    public async Task<UserResponseDto?> GetUserById(int id)
    {
        return await _context.Users
            .Where(u => u.Id == id)
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .Select(u => new UserResponseDto(
                u.Id,
                u.Username,
                u.UserRoles.Select(ur => ur.Role.Name).ToList()
            ))
            .FirstOrDefaultAsync();
    }

    public async Task UpdateUser(int id, string? username)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null && username != null) { user.Username = username; await _context.SaveChangesAsync(); }
    }

    public async Task DeleteUser(int id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null) { _context.Users.Remove(user); await _context.SaveChangesAsync(); }
    }
}
