using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

public static class RolePrivilegeSeeder
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();

        // 1. Seed Privileges
        if (!context.Privileges.Any())
        {
            var privileges = new List<Privilege>
            {
                // User Management
                new Privilege { NameSpace = "users.create", Module = "Users", Submodule="Create", Description = "Create new users" },
                new Privilege { NameSpace = "users.list", Module = "Users", Submodule="List", Description = "List users" },
                new Privilege { NameSpace = "users.read", Module = "Users", Submodule="Read", Description = "Read user data" },
                new Privilege { NameSpace = "users.update", Module = "Users", Submodule="Update", Description = "Update existing users" },
                new Privilege { NameSpace = "users.delete", Module = "Users", Submodule="Delete", Description = "Delete users" },

                // Role Management
                new Privilege { NameSpace = "roles.create", Module = "Roles", Submodule="Create", Description = "Create new roles" },
                new Privilege { NameSpace = "roles.list", Module = "Roles", Submodule="List", Description = "List role data" },
                new Privilege { NameSpace = "roles.read", Module = "Roles", Submodule="Read", Description = "Read role data" },
                new Privilege { NameSpace = "roles.update", Module = "Roles", Submodule="Update", Description = "Update existing roles" },
                new Privilege { NameSpace = "roles.delete", Module = "Roles", Submodule="Delete", Description = "Delete roles" },
                new Privilege { NameSpace = "roles.create", Module = "Roles", Submodule="Create", Description = "Create new roles" },

                new Privilege { NameSpace = "privileges.list", Module = "Privileges", Submodule="List", Description = "List privileges data" },
                new Privilege { NameSpace = "privileges.read", Module = "Privileges", Submodule="Read", Description = "Read privileges data" },
                new Privilege { NameSpace = "privileges.update", Module = "Privileges", Submodule="Update", Description = "Update existing privileges" },
                new Privilege { NameSpace = "privileges.delete", Module = "Privileges", Submodule="Delete", Description = "Delete privileges" },
            };
            context.Privileges.AddRange(privileges);
            context.SaveChanges();
        }

        // 2. Seed Roles
        if (!context.Roles.Any())
        {
            var roles = new List<Role>
            {
                new Role { Name = "SuperAdmin" },
                new Role { Name = "Admin" },
                new Role { Name = "User" }
            };
            context.Roles.AddRange(roles);
            context.SaveChanges();
        }

        if (!context.RolePrivileges.Any())
        {
            var allPrivileges = context.Privileges.ToList();
            var superAdminRole = context.Roles.Single(r => r.Name == "SuperAdmin");
            var adminRole = context.Roles.Single(r => r.Name == "Admin");
            var userRole = context.Roles.Single(r => r.Name == "User");

            foreach (var privilege in allPrivileges)
            {
                context.RolePrivileges.Add(new RolePrivilege { RoleId = superAdminRole.Id, PrivilegeId = privilege.Id });
            }

            var adminPrivileges = allPrivileges.Where(p => p.Module == "Users").ToList();
             adminPrivileges.Add(allPrivileges.Single(p=> p.NameSpace == "roles.read"));
            foreach (var privilege in adminPrivileges)
            {
                context.RolePrivileges.Add(new RolePrivilege { RoleId = adminRole.Id, PrivilegeId = privilege.Id });
            }

            var userPrivileges = allPrivileges.Where(p => p.NameSpace == "users.read").ToList();
            foreach (var privilege in userPrivileges)
            {
                context.RolePrivileges.Add(new RolePrivilege { RoleId = userRole.Id, PrivilegeId = privilege.Id });
            }

            context.SaveChanges();
        }

        if (!context.Users.Any(u => u.Username == "superadmin"))
        {
            var superAdminRole = context.Roles.Single(r => r.Name == "SuperAdmin");
            var superAdminUser = new User
            {
                Username = "superadmin",
                Password = PasswordHasher.Hash("password")
            };
            context.Users.Add(superAdminUser);
            context.SaveChanges();

            context.UserRoles.Add(new UserRole { UserId = superAdminUser.Id, RoleId = superAdminRole.Id });
            context.SaveChanges();
        }
    }
}
