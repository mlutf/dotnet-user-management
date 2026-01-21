
public class Privilege
{
    public int Id { get; set; }
    public string NameSpace { get; set; } = null!;
    public string? Module { get; set; }
    public string? Submodule { get; set; }
    public string? Description { get; set; }

    // Navigation property for the many-to-many relationship with Role
    public ICollection<RolePrivilege> RolePrivileges { get; set; } = new List<RolePrivilege>();
}