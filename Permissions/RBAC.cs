using Kissarekisteri.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Kissarekisteri.RBAC
{
    public enum RoleType
    {
        Admin,
        EventOrganizer
    }
    public enum PermissionType
    {
        CreateEvent,
        DeleteEvent,
        CreateCat,
        EditCat
    }

    public static class PermissionSeed
    {
        public static List<Permission> GetSeedData()
        {
            return Enum.GetValues(typeof(PermissionType))
                .Cast<PermissionType>()
                .Select(permissionType => new Permission { Name = permissionType.ToString() })
                .ToList();
        }
    }

    public static class RoleSeed
    {
        public static List<Role> GetSeedData()
        {
            return Enum.GetValues(typeof(RoleType))
                .Cast<RoleType>()
                .Select(roleType => new Role { Name = roleType.ToString() })
                .ToList();
        }
    }

    public class RoleSeedData
    {
        public string Name { get; set; }
        public List<PermissionType> Permissions { get; set; } = new List<PermissionType>();
    }

    public static class RolePermissionSeed
    {
        public static List<RoleSeedData> GetSeedData()
        {
            var roles = new List<RoleSeedData>
            {
                new RoleSeedData {
                    Name = RoleType.Admin.ToString(),
                    Permissions =
                    {
                        PermissionType.CreateEvent,
                        PermissionType.DeleteEvent,
                        PermissionType.CreateCat,
                        PermissionType.EditCat
                    }
                },
                new RoleSeedData
                {
                    Name = RoleType.EventOrganizer.ToString(),
                    Permissions =
                    [
                        PermissionType.CreateEvent,
                        PermissionType.DeleteEvent

                    ]
                }
            };

            return roles;
        }
    }
}

