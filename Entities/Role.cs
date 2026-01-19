
public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    public ICollection<RolePrivilege> RolePrivileges { get; set; } = new List<RolePrivilege>();
}