using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

public class PrivilegeService
{
    DbContext _context;
    public PrivilegeService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<PrivilegeDetailResponseDto> CreatePrivilege(PrivilegeRequestDto dto)
    {
        var privilege = new Privilege
        {
            NameSpace = dto.NameSpace,
            Description = dto.Description,
            Module = dto.Module,
            Submodule = dto.Submodule
        };
        await _context.AddAsync(privilege);
        await _context.SaveChangesAsync();
        return new PrivilegeDetailResponseDto(privilege.Id, privilege.NameSpace, privilege.Module, privilege.Submodule, privilege.Description);
    }

    public async Task<(List<PrivilegeDetailResponseDto>, int)> GetPrivilegesWithFilter(int skip = 0, int limit = 10)
    {
        IQueryable<Privilege> query = _context.Set<Privilege>();
        query = query.Skip(skip).Take(limit);
        var privileges = await query.ToListAsync();
        var totalCount = await _context.Set<Privilege>().CountAsync();

        var privilegeDtos = privileges.Select(p => new PrivilegeDetailResponseDto(p.Id, p.NameSpace, p.Module ?? "", p.Submodule, p.Description)).ToList();

        return (privilegeDtos, totalCount);
    }

    public async Task<PrivilegeDetailResponseDto?> GetPrivilegeById(int id)
    {
        var privilege = await _context.FindAsync<Privilege>(id);
        if (privilege == null) return null;

        return new PrivilegeDetailResponseDto(privilege.Id, privilege.NameSpace, privilege.Module, privilege.Submodule, privilege.Description);
    }

    public async Task<PrivilegeDetailResponseDto?> UpdatePrivilege(int id, PrivilegeRequestDto dto)
    {
        var privilege = await _context.FindAsync<Privilege>(id);
        if (privilege != null)
        {
            privilege.NameSpace = dto.NameSpace;
            privilege.Description = dto.Description;
            privilege.Module = dto.Module;
            privilege.Submodule = dto.Submodule;
            await _context.SaveChangesAsync();
            return new PrivilegeDetailResponseDto(privilege.Id, privilege.NameSpace, privilege.Module, privilege.Submodule, privilege.Description);
        }
        return null;
    }

    public async Task<PrivilegeDetailResponseDto?> DeletePrivilege(int id)
    {
        var privilege = await GetPrivilegeById(id);
        if (privilege != null)
        {
            _context.Remove(privilege);
            await _context.SaveChangesAsync();
            return privilege;
        }
        return null;
    }
}