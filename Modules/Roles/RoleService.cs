using Microsoft.EntityFrameworkCore;

public class RoleService
{
    private readonly AppDbContext _context;

    public RoleService(AppDbContext context) => _context = context;

    public async Task<RoleListResponseDto> CreateRole(string name)
    {
        var role = new Role { Name = name };
        _context.Roles.Add(role);
        await _context.SaveChangesAsync();
        return new RoleListResponseDto(role.Id, role.Name);
    }

    public async Task<List<RoleListResponseDto>> GetRoles()
    {
        return await _context.Roles
            .Select(r => new RoleListResponseDto(r.Id, r.Name))
            .ToListAsync();
    }

    public async Task<RoleResponseDto> GetOneById(int id)
    {
        var role = await _context.Roles
        .Include(r => r.RolePrivileges)
            .ThenInclude(rp => rp.Privilege)
        .FirstOrDefaultAsync(r => r.Id == id);

        if (role == null)
            throw new Exception("Role not found");

        return new RoleResponseDto(
            role.Id,
            role.Name,
            role.RolePrivileges
                .Select(rp => new PrivilegeResponseDto(
                    rp.Privilege.Id,
                    rp.Privilege.NameSpace,
                    rp.Privilege.Module ?? string.Empty,
                    rp.Privilege.Submodule,
                    rp.Privilege.Description
                ))
                .ToList()
        );

    }

    public async Task UpdateRole(int id, string? name)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role != null && name != null) { role.Name = name; await _context.SaveChangesAsync(); }
    }

    public async Task DeleteRole(int id)
    {
        var role = await _context.Roles.FindAsync(id);
        if (role != null) { _context.Roles.Remove(role); await _context.SaveChangesAsync(); }
    }
}