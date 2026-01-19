
public class RolePrivilege
{
    public int RoleId { get; set; }
    public Role Role { get; set; } = null!;

    public int PrivilegeId { get; set; }
    public Privilege Privilege { get; set; } = null!;
}