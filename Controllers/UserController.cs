using Kissarekisteri.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;


namespace Kissarekisteri.Controllers;

public class UserController(UserService userService) : Controller
{
    private readonly UserService _userService = userService;

    [HttpGet("signout")]
    public IActionResult Logout()
    {
        var callbackUrl = Url.Action(nameof(SignedOut), "Account", values: null, protocol: Request.Scheme);
        return SignOut(
            new AuthenticationProperties { RedirectUri = callbackUrl },
            CookieAuthenticationDefaults.AuthenticationScheme,
            OpenIdConnectDefaults.AuthenticationScheme);
    }

    public IActionResult SignedOut()
    {
        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    [HttpGet("login")]
    public async Task<IActionResult> Login()
    {
        return Ok("ok");
    }


    [HttpGet("users/{userId}")]
    public async Task<ActionResult<UserResponse>> GetUser([FromRoute] string userId)
    {
        var user = await _userService.GetUserById(userId);
        return Json(user);
    }

    [HttpGet("users")]
    public async Task<ActionResult<List<UserResponse>>> GetUsers()
    {
        var users = await _userService.GetUsers();
        return Json(users);
    }

    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userService.GetUserById(userId);
        return Json(user);
    }

    [HttpPost("users")]
    public async Task<ActionResult<UserResponse>> CreateUser()
    {
        var user = await _userService.CreateUser();
        return Json(user);
    }


    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [HttpPost("avatar")]
    public async Task<ActionResult<UserResponse>> UploadUserAvatar(IFormFile file)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userService.UploadUserPhotoAsync(userId, file);
        return Json(user);
    }
}

