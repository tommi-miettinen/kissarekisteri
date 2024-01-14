using Kissarekisteri.Database;
using Kissarekisteri.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kissarekisteri.Services;

public class PermissionService(KissarekisteriDbContext dbContext)
{
    public async Task<List<Permission>> GetPermissions(string userId)
    {
        var userRoleIds = await dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        return await dbContext.RolePermissions
            .Where(rp => userRoleIds.Contains(rp.RoleId))
            .Select(rp => rp.Permission)
            .Distinct()
            .ToListAsync();
    }

    public async Task<bool> HasPermission(string userId, string permissionName)
    {
        var permissions = await GetPermissions(userId);

        return permissions.Any(permission => permission.Name == permissionName);
    }

    public async Task<UserRole> GetUserRole(string userId)
    {
        try
        {
            var userRole = await dbContext.UserRoles
                .Where(ur => ur.UserId == userId)
                .Include(ur => ur.Role)
                .FirstOrDefaultAsync();

            return userRole;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task AssignRole(string userId, int roleId)
    {
        var userRole = await GetUserRole(userId);
        var role = await GetRoleById(roleId);

        if (userRole != null && role != null)
        {
            dbContext.UserRoles.Remove(userRole);
            dbContext.UserRoles.Add(new UserRole
            {
                UserId = userId,
                RoleId = role.Id,
                RoleName = role.Name
            });
        }

        await dbContext.SaveChangesAsync();
    }

    public async Task<Role> GetRoleById(int roleId)
    {
        var role = await dbContext.Roles.FindAsync(roleId);
        return role;
    }

    public async Task<List<Role>> GetRoles()
    {
        return await dbContext.Roles.ToListAsync();
    }
}
