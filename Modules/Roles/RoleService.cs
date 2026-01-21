using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public class RoleService
{
    protected readonly AppDbContext _context;

    public RoleService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<RoleResponseDto?> CreateRole(RoleRequestDto dto)
    {
        var role = new Role { Name = dto.Name };

        if (dto.PrivilegeIds != null)
        {
            var existingPrivilegeIds = await _context.Privileges.Where(p => dto.PrivilegeIds.Contains(p.Id)).Select(p => p.Id).ToListAsync();
            foreach (var privilegeId in existingPrivilegeIds)
            {
                role.RolePrivileges.Add(new RolePrivilege { PrivilegeId = privilegeId });
            }
        }

        await _context.AddAsync(role);
        await _context.SaveChangesAsync();
        return await GetRoleByIdWithPrivileges(role.Id);
    }

    public async Task<(List<RoleResponseDto>, int)> GetRolesWithPrivileges(int skip = 0, int limit = 10)
    {
        IQueryable<Role> query = _context.Roles
            .Include(r => r.RolePrivileges)
            .ThenInclude(rp => rp.Privilege);
        query = query.Skip(skip).Take(limit);

        var roles = await query.ToListAsync();
        var totalCount = await _context.Roles.CountAsync();

        var roleDtos = roles.Select(role => new RoleResponseDto(
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
        )).ToList();

        return (roleDtos, totalCount);
    }

    public async Task<RoleResponseDto?> GetRoleByIdWithPrivileges(int id)
    {
        var role = await _context.Roles
            .Include(r => r.RolePrivileges)
            .ThenInclude(rp => rp.Privilege)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (role == null)
            return null;

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

    public async Task<RoleResponseDto?> UpdateRole(int id, RoleRequestDto dto)
    {
        var role = await _context.Roles.Include(r => r.RolePrivileges).FirstOrDefaultAsync(r => r.Id == id);
        if (role != null)
        {
            role.Name = dto.Name;

            if (dto.PrivilegeIds != null)
            {
                var existingPrivilegeIds = role.RolePrivileges.Select(rp => rp.PrivilegeId).ToList();
                var privilegesToRemove = role.RolePrivileges.Where(rp => !dto.PrivilegeIds.Contains(rp.PrivilegeId)).ToList();
                _context.RolePrivileges.RemoveRange(privilegesToRemove);

                var privilegeIdsToAdd = dto.PrivilegeIds.Where(privilegeId => !existingPrivilegeIds.Contains(privilegeId)).ToList();
                var existingPrivileges = await _context.Privileges.Where(p => privilegeIdsToAdd.Contains(p.Id)).Select(p => p.Id).ToListAsync();
                foreach (var privilegeId in existingPrivileges)
                {
                    role.RolePrivileges.Add(new RolePrivilege { PrivilegeId = privilegeId });
                }
            }

            await _context.SaveChangesAsync();
            return await GetRoleByIdWithPrivileges(id);
        }
        return null;
    }

    public async Task<RoleResponseDto?> DeleteRole(int id)
    {
        var role = await GetRoleByIdWithPrivileges(id);
        if (role != null)
        {
            await _context.Roles.Where(r => r.Id == id).ExecuteDeleteAsync();
            return role;
        }
        return null;
    }
}