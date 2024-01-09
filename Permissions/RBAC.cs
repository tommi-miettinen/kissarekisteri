using Kissarekisteri.Models;
using System;
using System.Collections.Generic;
using System.Linq;


namespace Kissarekisteri.RBAC
{
    public enum RoleType
    {
        User,
        Admin,
        EventOrganizer
    }
    public enum PermissionType
    {
        CreateCatShowResult,
        CreateEvent,
        EditEvent,
        DeleteEvent,
        CreateCat,
        EditCat,
        AddAdminRole,
        AddEventOrganizerRole
    }

    public static class Permissions
    {
        public static List<Permission> GetPermissions()
        {
            return Enum.GetValues(typeof(PermissionType))
                .Cast<PermissionType>()
                .Select(permissionType => new Permission { Name = permissionType.ToString() })
                .ToList();
        }
    }

    public static class Roles
    {
        public static List<Role> GetRoles()
        {
            return Enum.GetValues(typeof(RoleType))
                .Cast<RoleType>()
                .Select(roleType => new Role { Name = roleType.ToString() })
                .ToList();
        }
    }


    public class RoleWithPermissions
    {
        public string Name { get; set; }
        public List<PermissionType> Permissions { get; set; } = [];
    }


    public static class RolePermissions
    {
        public static readonly List<RoleWithPermissions> RolesWithPermissions =
            [
                   new()
                   {
                       Name = RoleType.User.ToString(),
                       Permissions = []
                   },
                new()
                {
                    Name = RoleType.Admin.ToString(),
                    Permissions =
                    [
                        PermissionType.CreateCatShowResult,
                        PermissionType.CreateEvent,
                        PermissionType.DeleteEvent,
                        PermissionType.CreateCat,
                        PermissionType.EditCat,
                        PermissionType.AddAdminRole,
                        PermissionType.AddEventOrganizerRole
                    ]
                },
                new()
                {
                    Name = RoleType.EventOrganizer.ToString(),
                    Permissions =
                    [
                        PermissionType.CreateCatShowResult,
                        PermissionType.CreateEvent,
                        PermissionType.DeleteEvent

                    ]
                }
            ];
    }
};