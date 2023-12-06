using Kissarekisteribackend.Database;
using Kissarekisteribackend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using User = Kissarekisteribackend.Models.User;


namespace Kissarekisteribackend.Controllers;
public class UserController : Controller
{
    private readonly KissarekisteriDbContext _dbContext;
    private readonly IConfiguration _config;
    private readonly UserService _userService;


    public UserController(
        KissarekisteriDbContext dbContext,
        IConfiguration config,
        UserService userService
    )
    {
        _dbContext = dbContext;
        _config = config;
        _userService = userService;
    }

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
    public async Task<IActionResult> GetUser([FromRoute] string userId)
    {
        var user = await _userService.GetUserById(userId);
        return Json(user);
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        var users = await _userService.GetUsers();
        return Json(users);
    }

    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [HttpGet("me")]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userService.GetUserById(userId);
        return Json(user);
    }

    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [HttpPost("avatar")]
    public async Task<IActionResult> UploadUserAvatar(IFormFile file)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userService.UploadUserPhotoAsync(userId, file);
        return Json(user);
    }


    [HttpPut("users/{userId}")]
    public IActionResult EditUser([FromRoute] string userId, [FromBody] User updatedUser)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return NotFound();
        }
        user.IsBreeder = updatedUser.IsBreeder;
        _dbContext.SaveChanges();

        return Ok(user);
    }
}

