using Microsoft.EntityFrameworkCore;

public class PrivilegeService
{
    private readonly AppDbContext _context;

    public PrivilegeService(AppDbContext context) => _context = context;

    public async Task<PrivilegeResponseDto> CreatePrivilege(string nameSpace, string description, string module, string submodule)
    {
        var privilege = new Privilege { NameSpace = nameSpace, Description = description, Module = module, Submodule = submodule };
        _context.Privileges.Add(privilege);
        await _context.SaveChangesAsync();
        return new PrivilegeResponseDto(privilege.Id, privilege.NameSpace, privilege.Module, privilege.Submodule, privilege.Description);
    }

    public async Task<List<PrivilegeResponseDto>> GetPrivileges()
    {
        return await _context.Privileges
            .Select(p => new PrivilegeResponseDto(p.Id, p.NameSpace, p.Module ?? "", p.Submodule, p.Description))
            .ToListAsync();
    }

    public async Task UpdatePrivilege(int id, string? nameSpace, string? description, string? module, string? submodule)
    {
        var privilege = await _context.Privileges.FindAsync(id);
        if (privilege != null)
        {
            if (nameSpace != null) privilege.NameSpace = nameSpace;
            if (module != null) privilege.Module = module;
            if (submodule != null) privilege.Submodule = submodule;
            if (description != null) privilege.Description = description;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeletePrivilege(int id)
    {
        var privilege = await _context.Privileges.FindAsync(id);
        if (privilege != null)
        {
            _context.Privileges.Remove(privilege);
            await _context.SaveChangesAsync();
        }
    }
}