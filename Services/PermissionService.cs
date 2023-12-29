using Kissarekisteri.Database;
using Kissarekisteri.Models;
using Kissarekisteri.RBAC;
using Microsoft.EntityFrameworkCore;
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

    public async Task<bool> HasPermission(string userId, PermissionType permissionName)
    {
        var permissions = await GetPermissions(userId);

        return permissions.Any(permission => permission.Name == permissionName.ToString());
    }
}
