using Kissarekisteri.DTOs;
using Kissarekisteri.ErrorHandling;
using Kissarekisteri.Models;
using Kissarekisteri.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Kissarekisteri.Controllers;

[Route("odata/users")]
public class UserController(UserService userService, PermissionService permissionService) : ODataController
{
    [Authorize]
    [HttpGet("{userId}/permissions")]
    public async Task<ActionResult<List<Permission>>> GetPermissionsByUserId(string userId)
    {
        var permissions = await permissionService.GetPermissions(userId);
        return Ok(permissions);
    }

    [HttpGet("{userId}")]
    [EnableQuery]
    public async Task<ActionResult<UserResponse>> GetUser([FromRoute] string userId)
    {
        var user = await userService.GetUserById(userId);
        List<UserResponse> users = [user];
        return Ok(user);
    }

    [HttpGet]
    [EnableQuery]
    public async Task<ActionResult<IQueryable<UserResponse>>> GetUsers()
    {
        var users = await userService.FetchUsersAsync();
        return Ok(users.AsQueryable());
    }

    [HttpGet("roles")]
    [EnableQuery]
    public ActionResult<IQueryable> GetRoles()
    {
        return Ok(permissionService.GetRoles());
    }

    [Authorize]
    [HttpGet("me")]
    [EnableQuery]
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await userService.GetUserById(userId);
        return Ok(user);
    }

    [Authorize]
    [HttpDelete("{userId}")]
    public async Task<ActionResult<Result<bool>>> DeleteUser([FromRoute] string userId)
    {
        var result = await userService.DeleteUserByIdAsync(userId);
        return result;
    }

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Result<UserResponse>>> CreateUser([FromBody] UserCreatePayloadDTO userPayload)
    {
        var result = await userService.CreateUser(userPayload);
        if (!result.IsSuccess)
        {
            return HttpStatusMapper.Map(result.Errors);
        }
        return Ok(result);
    }

    [Authorize]
    [HttpPatch("{userId}")]
    public async Task<ActionResult<Result<UserResponse>>> UpdateUser([FromBody] UserUpdateRequestDTO userPayload)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await userService.UpdateUser(userId, userPayload);
        if (!result.IsSuccess)
        {
            return HttpStatusMapper.Map(result.Errors);
        }
        return Ok(result);
    }

    [Authorize]
    [HttpPost("avatar")]
    public async Task<ActionResult<UserResponse>> UploadUserAvatar(IFormFile file)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await userService.UploadUserPhotoAsync(userId, file);
        return Ok(user);
    }
}

