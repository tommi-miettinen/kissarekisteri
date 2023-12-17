using Kissarekisteri.Database;
using Microsoft.AspNetCore.Http;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kissarekisteri.Services;


public class UserResponse
{
    public string Id { get; set; }
    public string DisplayName { get; set; }
    public string GivenName { get; set; }
    public string Surname { get; set; }
    public string AvatarUrl { get; set; }
}

public class UserService(GraphServiceClient graphClient, UploadService uploadService, KissarekisteriDbContext dbContext)
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
                requestConfiguration.QueryParameters.Select = new string[] { "givenName", "surname", "id", "displayName", avatarUrl };

            });

            var response = users.Value.Select(u => new UserResponse
            {
                GivenName = u.GivenName,
                Id = u.Id,
                DisplayName = u.DisplayName,
                Surname = u.Surname,
                AvatarUrl = u.AdditionalData.TryGetValue(avatarUrl, out object value) ? value.ToString() : null
            }).ToList();

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

    public async Task<User> CreateUser()
    {
        try
        {
            string extensionAppId = "a1caf0508ec746569ef7e0fe9a263127";
            string avatarUrl = $"extension_{extensionAppId}_avatarUrl";

            var user = await _graphClient.Users.PostAsync(new User
            {
                AccountEnabled = true,
                MailNickname = "testuser",
                GivenName = "Test",
                Surname = "User",
                DisplayName = "Test User",
                PasswordProfile = new()
                {
                    Password = "Test1234",
                    ForceChangePasswordNextSignIn = false,
                },
                Identities =
                [
                    new()
                    {
                        SignInType = "emailAddress",
                        Issuer = "kissarekisteri.onmicrosoft.com",
                        IssuerAssignedId = "test@example.com"

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
                new string[] { "givenName", "surname", "id", "displayName", avatarUrl };
            });

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
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(ex.Message);
            Console.ResetColor();
            return null;
        }
    }
}
