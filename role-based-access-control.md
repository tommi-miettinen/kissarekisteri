# Role-Based Access Control

### Components


#### Data

```C#
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
```

#### Roles

```C#
public class Role
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<UserRole> UserRoles { get; set; }
}
```

#### Permissions

```C#
public class Permission
{
    public int Id { get; set; }
    public string Name { get; set; }
}
```

#### Role permissions

A lookup table for checking what permissions a role grants

```C#
    public class RolePermission
    {
        public int Id { get; set; }

        public string RoleName { get; set; }
        public int RoleId { get; set; }
        public string PermissionName { get; set; }
        public int PermissionId { get; set; }

        public Role Role { get; set; }
        public Permission Permission { get; set; }
    }
```

#### User roles

A lookup table for checking what roles the user has assigned

```C#
    public class UserRole
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
```

## Lookup flow
- Checking what roles the user has assigned
- Looking up Role permissions what permissions the role grants
- Does the permission allow for the action

## Lookup implementation

```C#
    public async Task<List<Permission>> GetPermissions(string userId)
    {
        var userRoleIds = await dbContext.UserRoles
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToListAsync();

        var rolePermissions = await dbContext.RolePermissions
            .Where(rp => userRoleIds.Contains(rp.RoleId))
            .Select(rp => rp.Permission)
            .ToListAsync();

        return rolePermissions;
    }

    public async Task<bool> HasPermission(string userId, string permissionName)
    {
        var permissions = await GetPermissions(userId);
        return permissions.Any(permission => permission.Name == permissionName);
    }
```

## Usage

```C#
 public async Task DoSomethingThatRequiresPermission(string userId)
 {
       var hasPermission = await permissionService.HasPermission(userId, Permissions.CatShowWrite);

    if (!hasPermission)
    {
        return null;
    }

// Do something
 }
```
