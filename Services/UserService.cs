using Kissarekisteri.Database;
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

    public async Task<List<UserResponse>> GetUsers()
    {
        var users = await graphClient.Users.GetAsync(requestConfiguration =>
        {
            requestConfiguration.QueryParameters.Select = new string[]
            {
            "givenName", "surname", "id", "displayName", "identities",
            };
        });

        var userIds = users.Value.Select(u => u.Id).ToList();
        var userInfos = await dbContext.UserInfos
                                       .Where(ui => userIds.Contains(ui.UserId))
                                       .ToListAsync();

        var responses = new List<UserResponse>();

        foreach (var u in users.Value)
        {
            var isMicrosoftAccount = u.Identities.FirstOrDefault()?.Issuer == "MicrosoftAccount";

            if (!isMicrosoftAccount)
            {
                var userInfo = userInfos.FirstOrDefault(ui => ui.UserId == u.Id);

                var userResponse = new UserResponse
                {
                    GivenName = u.GivenName,
                    Id = u.Id,
                    DisplayName = u.DisplayName,
                    Surname = u.Surname,
                    Email = u.Identities.FirstOrDefault()?.IssuerAssignedId,
                    AvatarUrl = userInfo?.AvatarUrl,
                    IsBreeder = userInfo?.IsBreeder ?? false,
                    UserRole = await permissionService.GetUserRole(u.Id) ?? null
                };

                responses.Add(userResponse);
            }
        }

        return responses;
    }

    public async Task<UserResponse> UploadUserPhotoAsync(string userId, IFormFile file)
    {
        var uploadedFile = await uploadService.UploadFile(file);


        var userInfo = await dbContext.UserInfos.FirstOrDefaultAsync(u => u.UserId == userId);
        if (userInfo != null)
        {
            userInfo.AvatarUrl = uploadedFile.Uri.AbsoluteUri;
        }
        else
        {
            var userInfoPayload = new UserInfo
            {
                AvatarUrl = uploadedFile.Uri.AbsoluteUri,
                UserId = userId
            };

            dbContext.UserInfos.Add(userInfoPayload);
        }

        await dbContext.SaveChangesAsync();

        var user = await GetUserById(userId);

        return user;
    }


    public async Task<Result<UserResponse>> UpdateUser(string userId, UserUpdateRequestDTO updatePayload)
    {
        var result = new Result<UserResponse>();
        var userInfo = await dbContext.UserInfos.FirstOrDefaultAsync(u => u.UserId == userId);

        if (userInfo != null)
        {
            userInfo.IsBreeder = updatePayload.IsBreeder;
            await dbContext.SaveChangesAsync();
        }

        var user = await GetUserById(userId);
        return result.Success(user);
    }

    public async Task<Result<User>> CreateUser(UserCreatePayloadDTO userPayload)
    {
        var result = new Result<User>();
        try
        {
            var user = await graphClient.Users.PostAsync(new User
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




            if (!string.IsNullOrEmpty(userPayload.Role))
            {
                var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == userPayload.Role);

                if (role != null)
                {
                    dbContext.UserRoles.Add(
                        new UserRole
                        {
                            UserId = user.Id,
                            RoleId = role.Id,
                            RoleName = role.Name.ToString()
                        }
                        );

                    await dbContext.SaveChangesAsync();
                }
            }

            return result.Success(user);
        }
        catch (Exception ex)
        {
            return result.AddError(new Error(ex.Message, "CreateFail"));
        }
    }

    public async Task<UserResponse> GetUserById(string userId)
    {
        try
        {
            var userInfo = await dbContext.UserInfos.FirstOrDefaultAsync(u => u.UserId == userId);

            if (userInfo != null)
            {
                userInfo.IsBreeder = userInfo.IsBreeder;
            }


            var user = await graphClient.Users[userId].GetAsync(requestConfiguration =>
            {
                requestConfiguration.QueryParameters.Select =
                new string[] { "givenName", "surname", "id", "displayName", "identities" };
            });

            var response = new UserResponse
            {
                GivenName = user.GivenName,
                Id = user.Id,
                DisplayName = user.DisplayName,
                Surname = user.Surname,
                Email = user.Identities.FirstOrDefault().IssuerAssignedId,
                AvatarUrl = userInfo?.AvatarUrl ?? null,
                IsBreeder = userInfo?.IsBreeder ?? false,
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
        await graphClient.Users[userId].DeleteAsync();

        var user = await GetUserById(userId);

        if (user != null)
        {
            return result.AddError(new Error("User deletion failed", "DeleteFail"));

        }

        return result.Success(true);
    }

    public async Task<UserRole> CreateUserRole(UserRole userRole)
    {
        dbContext.UserRoles.Add(userRole);
        await dbContext.SaveChangesAsync();
        return userRole;
    }
}
