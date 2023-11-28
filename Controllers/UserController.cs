using Kissarekisteribackend.Database;
using Kissarekisteribackend.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Security.Claims;

namespace Kissarekisteribackend.Controllers;
public class UserController : Controller
{
    private readonly KissarekisteriDbContext _dbContext;
    private readonly IConfiguration _config;

    public UserController(KissarekisteriDbContext dbContext, IConfiguration config)
    {
        _dbContext = dbContext;
        _config = config;
    }

    [Authorize]
    [HttpGet("logintest")]
    public IActionResult Login()
    {
        return Ok("logged in");
    }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("moro");
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
    [HttpGet("claims")]
    public IActionResult GetClaims()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        return Json(userId);
    }

    [HttpPost("users")]
    public IActionResult CreateUser([FromBody] User userPayload)
    {
        var user = _dbContext.Users.Add(userPayload).Entity;
        _dbContext.SaveChanges();

        return Json(user);
    }

    [HttpGet("users/{userId}")]
    public IActionResult GetUser([FromRoute] string userId)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("users")]
    public IActionResult GetUsers()
    {
        var users = _dbContext.Users.ToList();
        return Ok(users);
    }

    [HttpGet("me")]
    public IActionResult GetCurrentUser()
    {

        var userId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
        var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

        return Ok(user);
    }


    [HttpPut("users/{userId}")]
    public IActionResult EditUser([FromRoute] string userId, [FromBody] User updatedUser)
    {
        var user = _dbContext.Users.FirstOrDefault(u => u.Id == userId);

        if (user == null)
        {
            return NotFound();
        }

        user.Username = updatedUser.Username;
        user.Email = updatedUser.Email;
        user.IsBreeder = updatedUser.IsBreeder;
        _dbContext.SaveChanges();

        return Ok(user);
    }
}

