﻿using Kissarekisteri.Database;
using Kissarekisteri.DTOs;
using Kissarekisteri.ErrorHandling;
using Kissarekisteri.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kissarekisteri.Services;

public class UserService(
    GraphServiceClient graphClient,
    UploadService uploadService,
    KissarekisteriDbContext dbContext,
    PermissionService permissionService
    )
{
    private readonly GraphServiceClient _graphClient = graphClient;
    private readonly UploadService _uploadService = uploadService;
    private readonly KissarekisteriDbContext _dbContext = dbContext;


    public async Task<List<UserResponse>> GetUsers()
    {
        try
        {
            string extensionAppId = "a1caf0508ec746569ef7e0fe9a263127";
            string avatarUrl = $"extension_{extensionAppId}_avatarUrl";

            var users = await _graphClient.Users.GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Select = new string[]
                {
                "givenName",
                "surname",
                "id",
                "displayName",
                "identities",
                avatarUrl
                };
            });

            var responses = new List<UserResponse>();

            foreach (var u in users.Value)
            {
                var userResponse = new UserResponse
                {
                    GivenName = u.GivenName,
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    Surname = u.Surname,
                    Email = u.Identities.FirstOrDefault().IssuerAssignedId,
                    AvatarUrl = u.AdditionalData.TryGetValue(avatarUrl, out object value) ? value.ToString() : null,
                    UserRole = await permissionService.GetUserRole(u.Id) ?? null
                };
                responses.Add(userResponse);
            }

            return responses;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
            return null;
        }
    }

    public async Task<UserResponse> UploadUserPhotoAsync(string userId, IFormFile file)
    {

        string extensionAppId = "a1caf0508ec746569ef7e0fe9a263127";
        string avatarUrl = $"extension_{extensionAppId}_avatarUrl";

        var uploadedFile = await _uploadService.UploadFile(file);
        var update = new Microsoft.Graph.Models.User
        {
            AdditionalData = new Dictionary<string, object>
            {
                [avatarUrl] = uploadedFile.Uri.AbsoluteUri
            }
        };

        var user = await _graphClient.Users[userId].PatchAsync(update);

        var response = new UserResponse
        {
            GivenName = user.GivenName,
            Id = user.Id,
            DisplayName = user.DisplayName,
            Surname = user.Surname,
            AvatarUrl = user.AdditionalData.TryGetValue(avatarUrl, out object value) ? value.ToString() : null
        };

        return response;
    }

    public async Task<User> CreateUser(UserCreatePayloadDTO userPayload)
    {
        try
        {
            string extensionAppId = "a1caf0508ec746569ef7e0fe9a263127";
            string avatarUrl = $"extension_{extensionAppId}_avatarUrl";

            var user = await _graphClient.Users.PostAsync(new User
            {
                AccountEnabled = true,
                MailNickname = userPayload.MailNickname,
                GivenName = userPayload.GivenName,
                Surname = userPayload.Surname,
                DisplayName = userPayload.DisplayName,
                PasswordProfile = new()
                {
                    Password = userPayload.Password,
                    ForceChangePasswordNextSignIn = false,
                },
                Identities =
                [
                    new()
                    {
                        SignInType = "emailAddress",
                        Issuer = "kissarekisteri.onmicrosoft.com",
                        IssuerAssignedId = userPayload.Email

                    }
                ],
                PasswordPolicies = "DisablePasswordExpiration",
            });

            _dbContext.UserRoles.Add(
                new Models.UserRole
                {
                    UserId = user.Id,
                    RoleId = 1
                }
                );


            return user;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
            return null;
        }
    }

    public async Task<UserResponse> GetUserById(string userId)
    {
        try
        {
            string extensionAppId = "a1caf0508ec746569ef7e0fe9a263127";
            string avatarUrl = $"extension_{extensionAppId}_avatarUrl";

            var user = await _graphClient.Users[userId].GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Select =
                new string[] { "givenName", "surname", "id", "displayName", "identities", avatarUrl };
            });

            var response = new UserResponse
            {
                GivenName = user.GivenName,
                Id = user.Id,
                DisplayName = user.DisplayName,
                Surname = user.Surname,
                Email = user.Identities.FirstOrDefault().IssuerAssignedId,
                AvatarUrl = user.AdditionalData.TryGetValue(avatarUrl, out object value) ? value.ToString() : null,
                UserRole = await permissionService.GetUserRole(user.Id) ?? null
            };

            return response;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
            return null;
        }
    }

    public async Task<Result<bool>> DeleteUserByIdAsync(string userId)
    {
        var result = new Result<bool>();
        await _graphClient.Users[userId].DeleteAsync();

        var user = await GetUserById(userId);

        if (user != null)
        {
            return result.AddError(new Error("User deletion failed", "DeleteFail"));

        }

        return result.Success(true);
    }

    public async Task<List<Role>> GetRoles()
    {
        var roles = await _dbContext.Roles.ToListAsync();
        return roles;
    }

    public async Task<UserRole> CreateUserRole(UserRole userRole)
    {
        _dbContext.UserRoles.Add(userRole);
        await _dbContext.SaveChangesAsync();
        return userRole;
    }
}
