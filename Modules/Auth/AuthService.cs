using Microsoft.EntityFrameworkCore;

public class AuthService
{
    private readonly AppDbContext _context;
    private readonly JwtService _jwtService;

    public AuthService(AppDbContext context, JwtService jwtService)
    {
        _context = context;
        _jwtService = jwtService;
    }

    public async Task<string?> Login(string username, string password)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePrivileges)
                        .ThenInclude(rp => rp.Privilege)
            .FirstOrDefaultAsync(u => u.Username == username);

        if (user == null || !PasswordHasher.Verify(password, user.Password))
            return null;

        var permissions = user.UserRoles
            .SelectMany(ur => ur.Role.RolePrivileges)
            .Select(rp => rp.Privilege.NameSpace)
            .Distinct()
            .ToList();

        return _jwtService.GenerateToken(user, permissions);
    }
}
