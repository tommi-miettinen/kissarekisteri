﻿using Kissarekisteri.DTOs;
using Kissarekisteri.Models;
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

public class UserController(
    UserService userService,
    CatService catService,
    SeedService seedService
    ) : Controller
{

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
    public IActionResult Login()
    {
        return Ok("ok");
    }


    [HttpGet("users/{userId}")]
    public async Task<ActionResult<UserResponse>> GetUser([FromRoute] string userId)
    {
        var user = await userService.GetUserById(userId);
        if (user == null)
        {
            return NotFound();
        }
        return Json(user);
    }

    [HttpGet("users")]
    public async Task<ActionResult<List<UserResponse>>> GetUsers()
    {
        var users = await userService.GetUsers();
        return Json(users);
    }

    [HttpPost("seed")]
    public async Task<ActionResult<List<UserResponse>>> SeedUsers()
    {
        var users = await seedService.SeedUsers();
        var cats = await seedService.SeedCats();
        await seedService.SeedCatShows();
        return Json(users);
    }

    /// <summary>
    /// Endpoint for getting the currently logged in user.
    /// </summary>
    /// <remarks>Needs a logged in user</remarks>
    /// <returns>The logged in user</returns>
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    [HttpGet("me")]
    public async Task<ActionResult<UserResponse>> GetCurrentUser()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await userService.GetUserById(userId);
        return Json(user);
    }

    [HttpPost("users")]
    public async Task<ActionResult<UserResponse>> CreateUser()
    {
        var user = await userService.CreateUser(new UserCreatePayloadDTO
        {

        });
        return Json(user);
    }

    /// <summary>
    /// Endpoint for uploading a user avatar.
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
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
    [HttpGet("/users/{userId}/cats")]
    public async Task<ActionResult<Cat>> GetCatsByUserId(string userId)
    {
        var catsByUserId = await catService.GetCatByUserIdAsync(userId);
        return Json(catsByUserId);
    }
}

