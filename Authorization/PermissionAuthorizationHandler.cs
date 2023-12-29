using Kissarekisteri.Services;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kissarekisteri.Authorization;


public class PermissionAuthorizeAttribute : AuthorizeAttribute
{
    public PermissionAuthorizeAttribute(RBAC.PermissionType permission) : base()
    {
        Policy = permission.ToString();
    }
}

public class PermissionRequirement(RBAC.PermissionType requiredPermission) : IAuthorizationRequirement
{
    public RBAC.PermissionType RequiredPermission { get; } = requiredPermission;
}

public class PermissionAuthorizationHandler(PermissionService permissionService)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement
    )
    {
        var userId = context.User.Claims.FirstOrDefault(
            c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine(userId);
        Console.ResetColor();

        if (userId == null)
        {
            context.Fail();
            return;
        }

        var hasPermission = await permissionService.HasPermission(
            userId,
            requirement.RequiredPermission
        );
        if (hasPermission)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }
    }
}
