using Azure.Identity;
using Azure.Storage.Blobs;
using ConsoleApplication;
using Kissarekisteri.Database;
using Kissarekisteri.SeedData;
using Kissarekisteri.Services;
using KissarekisteriConsole.ConsoleApplication.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace KissarekisteriConsole.ConsoleApplication;

public class Program
{
    public static async Task Main()
    {
        string[] envOptions = { "Development", "Production" };

        int selectedOption = KeyboardNavigation.GetMenuChoice("Choose Environment", envOptions);
        var env = envOptions[selectedOption];

        Assembly assembly = Assembly.Load("Kissarekisteri");

        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets(assembly)
            .Build();

        var instance = config["AzureAdB2C:Instance"];
        var domain = config["AzureAdB2C:Domain"];
        var policyName = config["AzureAdB2C:SignUpSignInPolicyId"];
        var appId = config["AzureAdB2C:ClientId"];
        var clientSecret = config["AzureAdB2C:ClientSecret"];
        var tenantId = config["AzureAdB2C:TenantId"];
        var dbConnectionString =
            env == "Development"
                ? config.GetConnectionString("developmentSQL")
                : config.GetConnectionString("AzureSQL");

        var clientSecretCredential = new ClientSecretCredential(domain, appId, clientSecret);
        var scopes = new[] { "https://graph.microsoft.com/.default" };
        var graphClient = new GraphServiceClient(clientSecretCredential, scopes);

        var options = new DbContextOptionsBuilder<KissarekisteriDbContext>()
            .UseSqlServer(dbConnectionString)
            .Options;
        var dbContext = new KissarekisteriDbContext(options);

        var blobServiceClient = new BlobServiceClient(config.GetConnectionString("Storage"));
        var uploadService = new UploadService(blobServiceClient);
        var permissionService = new PermissionService(dbContext);
        var userService = new UserService(graphClient, uploadService, dbContext, permissionService);
        var catService = new CatService(dbContext, uploadService, userService);
        var catShowService = new CatShowService(dbContext, uploadService, permissionService);
        var seedService = new SeedService(userService, catService, catShowService, dbContext);

        /*
        await seedService.SeedPermissions();
        await seedService.SeedRoles();
        await seedService.SeedRolePermissions();
        */

        while (true)
        {
            var actionOptions = new string[]
            {
                "1. Assign Role to User",
                "2. Seed Cat Relationships",
                "3. Update Cat Photos",
                "4. Update Cat Show Photos",
                "5. Exit",
            };
            var selectedAction = KeyboardNavigation.GetMenuChoice("Select action", actionOptions);

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            string Json(object obj) => JsonSerializer.Serialize(obj, jsonSerializerOptions);

            switch (selectedAction)
            {
                case 0:
                    var userss = await userService.GetUsers();
                    var roles = await permissionService.GetRoles();

                    var selectedUserIndex = KeyboardNavigation.GetMenuChoice(
                        "Select user",
                        userss.Select(u => u.Email).ToArray()
                    );
                    var selectedUser = userss[selectedUserIndex];

                    var selectedRoleIndex = KeyboardNavigation.GetMenuChoice(
                        "Select role",
                        roles.Select(r => r.Name).ToArray()
                    );
                    var selectedRole = roles[selectedRoleIndex];

                    if (selectedUser == null || selectedRole == null)
                    {
                        Console.WriteLine("Invalid input. Please choose a valid action.");
                        break;
                    }

                    var confirmOptions = new string[] { "Yes", "No" };
                    var selectedConfirmOption = KeyboardNavigation.GetMenuChoice(
                        $"Assign {selectedRole.Name} to {selectedUser.Email}?",
                        confirmOptions
                    );
                    var confirmed = confirmOptions[selectedConfirmOption] == "Yes";

                    if (confirmed)
                    {
                        await permissionService.AssignRoleWithoutPermissionCheck(
                            selectedUser.Id,
                            selectedRole.Id
                        );
                        Console.Clear();
                        Console.WriteLine("Role assigned.");
                        Thread.Sleep(2000);
                        break;
                    }

                    break;

                case 1:
                    await seedService.SeedCatRelations();
                    Console.Clear();
                    Console.WriteLine("Relationships added.");
                    Thread.Sleep(2000);
                    break;

                case 2:
                    var cats = await dbContext.Cats.ToListAsync();

                    foreach (var cat in cats)
                    {
                        cat.ImageUrl = CatImageUrls.ImageUrls[
                            new Random().Next(0, CatImageUrls.ImageUrls.Count)
                        ];
                    }

                    await dbContext.SaveChangesAsync();

                    break;

                case 3:
                    await seedService.SeedCatShows(true, 50);

                    break;

                default:
                    return;
            }
        }
    }
}
