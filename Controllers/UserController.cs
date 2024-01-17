using Kissarekisteri.DTOs;
using Kissarekisteri.ErrorHandling;
using Kissarekisteri.Models;
using Kissarekisteri.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Kissarekisteri.Controllers;

[ApiController]
[Route("api/users")]
public class UserController(
    UserService userService,
    CatService catService,
    PermissionService permissionService
    ) : Controller
{

    [Authorize]
    [HttpGet("{userId}/permissions")]
    public async Task<ActionResult<List<Permission>>> GetPermissionsByUserId(string userId)
    {
        var permissions = await permissionService.GetPermissions(userId);
        return Json(permissions);
    }


    [HttpGet("{userId}")]
    public async Task<ActionResult<UserResponse>> GetUser([FromRoute] string userId)
    {
        var user = await userService.GetUserById(userId);
        if (user == null)
        {
            return NotFound();
        }
        return Json(user);
    }

    [HttpGet]
    public async Task<ActionResult<List<UserResponse>>> GetUsers()
    {
        var users = await userService.GetUsers();
        return Json(users);
    }

    [HttpGet("roles")]
    public async Task<ActionResult<List<Role>>> GetRoles()
    {
        var roles = await permissionService.GetRoles();
        return Json(roles);
    }

    /// <summary>
    /// Endpoint for getting the currently logged in user.
    /// </summary>
    /// <remarks>Needs a logged in user</remarks>
    /// <returns>The logged in user</returns>
    [Authorize]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await userService.GetUserById(userId);
        return Json(user);
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
        return Json(result);
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
        return Json(result);
    }


    /// <summary>
    /// Endpoint for uploading a user avatar.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [Authorize]
    [HttpPost("avatar")]
    public async Task<ActionResult<UserResponse>> UploadUserAvatar(IFormFile file)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await userService.UploadUserPhotoAsync(userId, file);
        return Json(user);
    }

    /// <summary>
    /// Gets all cats owned by a user.
    /// </summary>
    /// <param name="userId">The ID of the user</param>
    /// <returns>The cats owned by the user</returns>
    [HttpGet("{userId}/cats")]
    public async Task<ActionResult<Cat>> GetCatsByUserId(string userId)
    {
        var catsByUserId = await catService.GetCatByUserIdAsync(userId);
        return Json(catsByUserId);
    }
}

