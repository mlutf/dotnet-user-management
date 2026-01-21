using Microsoft.EntityFrameworkCore;

public class UserService
{

    private readonly AppDbContext _context;
    public UserService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<UserDetailResponseDto?> CreateUser(UserRequestDto dto)
    {
        if (dto.Password == null)
        {
            throw new ArgumentNullException(nameof(dto.Password));
        }
        var user = new User { Username = dto.Username, Password = PasswordHasher.Hash(dto.Password) };

        if (dto.RoleIds != null)
        {
            var existingRoleIds = await _context.Roles.Where(r => dto.RoleIds.Contains(r.Id)).Select(r => r.Id).ToListAsync();
            foreach (var roleId in existingRoleIds)
            {
                user.UserRoles.Add(new UserRole { RoleId = roleId });
            }
        }

        await _context.AddAsync(user);
        await _context.SaveChangesAsync();
        return await GetUserByIdWithRoles(user.Id);
    }

    public async Task<(List<UserDetailResponseDto>, int)> GetUsersWithRoles(int skip = 0, int limit = 10, string? username = null)
    {
        IQueryable<User> query = _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role);

        if (!string.IsNullOrEmpty(username))
        {
            query = query.Where(u => u.Username.Contains(username));
        }

        var totalCount = await query.CountAsync();
        var users = await query.Skip(skip).Take(limit).ToListAsync();

        var userDtos = users.Select(u => new UserDetailResponseDto(
            u.Id,
            u.Username,
            u.UserRoles.Select(ur => ur.Role.Name).ToList()
        )).ToList();

        return (userDtos, totalCount);
    }

    public async Task<UserDetailResponseDto?> GetUserByIdWithRoles(int id)
    {
        var user = await _context.Users
            .Include(u => u.UserRoles)
            .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == id);

        if (user == null) return null;

        return new UserDetailResponseDto(
            user.Id,
            user.Username,
            user.UserRoles.Select(ur => ur.Role.Name).ToList()
        );
    }

    public async Task<UserDetailResponseDto?> UpdateUser(int id, UserRequestDto dto)
    {
        var user = await _context.Users.Include(u => u.UserRoles).FirstOrDefaultAsync(u => u.Id == id);
        if (user != null)
        {
            if (dto.Username != null)
            {
                user.Username = dto.Username;
            }

            if (dto.Password != null)
            {
                user.Password = PasswordHasher.Hash(dto.Password);
            }

            if (dto.RoleIds != null)
            {
                var existingRoleIds = user.UserRoles.Select(ur => ur.RoleId).ToList();
                var rolesToRemove = user.UserRoles.Where(ur => !dto.RoleIds.Contains(ur.RoleId)).ToList();
                _context.UserRoles.RemoveRange(rolesToRemove);

                var roleIdsToAdd = dto.RoleIds.Where(roleId => !existingRoleIds.Contains(roleId)).ToList();
                var existingRoles = await _context.Roles.Where(r => roleIdsToAdd.Contains(r.Id)).Select(r => r.Id).ToListAsync();
                foreach (var roleId in existingRoles)
                {
                    user.UserRoles.Add(new UserRole { RoleId = roleId });
                }
            }

            await _context.SaveChangesAsync();
            return await GetUserByIdWithRoles(id);
        }
        return null;
    }

    public async Task<UserDetailResponseDto?> DeleteUser(int id)
    {
        var user = await GetUserByIdWithRoles(id);
        if (user != null)
        {
            await _context.Users.Where(u => u.Id == id).ExecuteDeleteAsync();
            return user;
        }
        return null;
    }
}
