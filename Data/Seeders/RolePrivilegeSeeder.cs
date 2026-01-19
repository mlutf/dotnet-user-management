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
                new Privilege { NameSpace = "users.create", Module = "Users", Description = "Create new users" },
                new Privilege { NameSpace = "users.read", Module = "Users", Description = "Read user data" },
                new Privilege { NameSpace = "users.update", Module = "Users", Description = "Update existing users" },
                new Privilege { NameSpace = "users.delete", Module = "Users", Description = "Delete users" },

                // Role Management
                new Privilege { NameSpace = "roles.create", Module = "Roles", Description = "Create new roles" },
                new Privilege { NameSpace = "roles.read", Module = "Roles", Description = "Read role data" },
                new Privilege { NameSpace = "roles.update", Module = "Roles", Description = "Update existing roles" },
                new Privilege { NameSpace = "roles.delete", Module = "Roles", Description = "Delete roles" },

                // Privilege Management
                new Privilege { NameSpace = "privileges.read", Module = "Privileges", Description = "Read privilege data" },
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

        // 3. Seed Role-Privilege Mappings
        if (!context.RolePrivileges.Any())
        {
            var allPrivileges = context.Privileges.ToList();
            var superAdminRole = context.Roles.Single(r => r.Name == "SuperAdmin");
            var adminRole = context.Roles.Single(r => r.Name == "Admin");
            var userRole = context.Roles.Single(r => r.Name == "User");

            // SuperAdmin gets all privileges
            foreach (var privilege in allPrivileges)
            {
                context.RolePrivileges.Add(new RolePrivilege { RoleId = superAdminRole.Id, PrivilegeId = privilege.Id });
            }

            // Admin gets user management privileges
            var adminPrivileges = allPrivileges.Where(p => p.Module == "Users").ToList();
             adminPrivileges.Add(allPrivileges.Single(p=> p.NameSpace == "roles.read"));
            foreach (var privilege in adminPrivileges)
            {
                context.RolePrivileges.Add(new RolePrivilege { RoleId = adminRole.Id, PrivilegeId = privilege.Id });
            }

            // User gets read-only access to users
            var userPrivileges = allPrivileges.Where(p => p.NameSpace == "users.read").ToList();
            foreach (var privilege in userPrivileges)
            {
                context.RolePrivileges.Add(new RolePrivilege { RoleId = userRole.Id, PrivilegeId = privilege.Id });
            }

            context.SaveChanges();
        }

        // 4. Seed SuperAdmin User
        if (!context.Users.Any(u => u.Username == "superadmin"))
        {
            var superAdminRole = context.Roles.Single(r => r.Name == "SuperAdmin");
            var superAdminUser = new User
            {
                Username = "superadmin",
                Password = PasswordHasher.Hash("password") // Use a secure password in production
            };
            context.Users.Add(superAdminUser);
            context.SaveChanges(); // Save to get the User's Id

            context.UserRoles.Add(new UserRole { UserId = superAdminUser.Id, RoleId = superAdminRole.Id });
            context.SaveChanges();
        }
    }
}
