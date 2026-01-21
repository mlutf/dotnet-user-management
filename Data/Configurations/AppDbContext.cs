using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Privilege> Privileges => Set<Privilege>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePrivilege> RolePrivileges => Set<RolePrivilege>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserRole>().HasKey(x => new { x.UserId, x.RoleId });
        modelBuilder.Entity<RolePrivilege>().HasKey(x => new { x.RoleId, x.PrivilegeId });

        // Configure User-UserRole relationship for cascade delete
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Role-UserRole relationship for cascade delete
        modelBuilder.Entity<UserRole>()
            .HasOne(ur => ur.Role)
            .WithMany(r => r.UserRoles)
            .HasForeignKey(ur => ur.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Role-RolePrivilege relationship for cascade delete
        modelBuilder.Entity<RolePrivilege>()
            .HasOne(rp => rp.Role)
            .WithMany(r => r.RolePrivileges)
            .HasForeignKey(rp => rp.RoleId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure Privilege-RolePrivilege relationship for cascade delete
        modelBuilder.Entity<RolePrivilege>()
            .HasOne(rp => rp.Privilege)
            .WithMany(p => p.RolePrivileges)
            .HasForeignKey(rp => rp.PrivilegeId)
            .OnDelete(DeleteBehavior.Cascade);


        base.OnModelCreating(modelBuilder);
    }
}
