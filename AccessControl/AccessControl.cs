using System.Collections.Generic;

namespace Kissarekisteri.AccessControl;

public static class Roles
{
    public const string Admin = "Admin";
    public const string EventOrganizer = "EventOrganizer";
    public const string User = "User";

    public static readonly List<string> All = [Admin, EventOrganizer, User];
}

public static class Permissions
{
    public const string CatShowWrite = "CatShow.Write";
    public const string RoleAdminWrite = "Role.Admin.Write";
    public const string RoleEventOrganizerWrite = "Role.EventOrganizer.Write";

    public static readonly List<string> All = [CatShowWrite, RoleAdminWrite, RoleEventOrganizerWrite];
}

public static class RolePermissions
{
    public static readonly Dictionary<string, List<string>> RolesWithPermissions = new()
    {
        [Roles.Admin] = [Permissions.CatShowWrite, Permissions.RoleAdminWrite, Permissions.RoleEventOrganizerWrite],
        [Roles.EventOrganizer] = [Permissions.CatShowWrite, Permissions.RoleEventOrganizerWrite],
        [Roles.User] = []
    };
}