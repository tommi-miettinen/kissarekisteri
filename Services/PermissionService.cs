using Kissarekisteri.Database;
using Kissarekisteri.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kissarekisteri.Services
{
    public class PermissionService(KissarekisteriDbContext dbContext)
    {
        private readonly KissarekisteriDbContext _dbContext = dbContext;

        public async Task<List<Permission>> GetPermissions(string userId)
        {
            var userRoles = await _dbContext.UserRoles
                .Where(userRoles => userRoles.UserId == userId)
                .ToListAsync();

            var userRoleIds = userRoles.Select(ur => ur.RoleId).ToList();

            var rolePermissions = await _dbContext.RolePermissions
                .Where(rolePermission => userRoleIds.Contains(rolePermission.RoleId))
                .ToListAsync();

            var permissionIds = rolePermissions.Select(rolePermission => rolePermission.PermissionId)
                .ToList();

            var permissions = await _dbContext.Permissions
                .Where(permission => permissionIds.Contains(permission.Id))
                .ToListAsync();

            return permissions;
        }
    }
}
