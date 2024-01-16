using Azure.Identity;
using Azure.Storage.Blobs;
using ConsoleApplication;
using Kissarekisteri.Database;
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
        string[] envOptions = ["Development", "Production"];

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
                "1. Create Roles",
                "2. Assign Role to User",
                "3. Seed Cats",
                "4. Seed Cat Shows",
                "5. Create Database",
                "6. Exit",
            };
            var selectedAction = KeyboardNavigation.GetMenuChoice("Select action", actionOptions) + 1;

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            string Json(object obj) => JsonSerializer.Serialize(obj, jsonSerializerOptions);

            bool confirmed;
            string[] confirmOptions;
            int selectedOptionIndex;

            switch (selectedAction)
            {
                case 1:
                    await seedService.SeedRoles();
                    await seedService.SeedPermissions();
                    await seedService.SeedRolePermissions();
                    break;

                case 2:
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

                    confirmOptions = ["Yes", "No"];
                    selectedOptionIndex = KeyboardNavigation.GetMenuChoice(
                        $"Assign {selectedRole.Name} to {selectedUser.Email}?",
                        confirmOptions
                    );

                    confirmed = confirmOptions[selectedOptionIndex] == "Yes";

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

                case 3:
                    await seedService.SeedCats(true, 50);
                    await seedService.SeedCatRelations();
                    await seedService.SeedCatPhotos();
                    Console.Clear();
                    Console.WriteLine("Cats seeded.");
                    Thread.Sleep(2000);
                    break;

                case 4:
                    await seedService.SeedCatShows(true, 50);
                    Console.Clear();
                    Console.WriteLine("Cat shows seeded.");
                    Thread.Sleep(2000);
                    break;

                case 5:
                    if (env == "Production")
                    {
                        Console.WriteLine($"Are you sure you want to create the database in {env}?");
                        confirmOptions = ["Yes", "No"];
                        selectedOptionIndex = KeyboardNavigation.GetMenuChoice(
                                                       $"Create database in {env}?",
                                                       confirmOptions
                                                                                                         );
                        confirmed = confirmOptions[selectedOptionIndex] == "Yes";

                        if (!confirmed)
                        {
                            break;
                        }
                    }

                    await dbContext.Database.EnsureCreatedAsync();
                    Console.Clear();
                    Console.WriteLine("Database created.");
                    Thread.Sleep(2000);
                    break;

                default:
                    return;
            }
        }
    }
}
