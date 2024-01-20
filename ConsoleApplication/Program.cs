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

namespace KissarekisteriConsole.ConsoleApplication;

public class Program
{
    public static async Task Main()
    {
        string[] envOptions = ["Development", "Production"];

        int selectedOption = KeyboardNavigation.GetMenuChoice("Choose Environment", envOptions);
        var env = envOptions[selectedOption];

        var config = new ConfigurationBuilder()
            .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets(Assembly.Load("Kissarekisteri"))
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
        var graphClient = new GraphServiceClient(clientSecretCredential, ["https://graph.microsoft.com/.default"]);

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


        string[] actionOptions = [
            "1. Create Roles",
            "2. Assign Role to User",
            "3. Seed Cats",
            "4. Seed Cat Shows",
            "5. Create Database",
            "6. Exit"
        ];

        while (true)
        {
            var selectedAction = KeyboardNavigation.GetMenuChoice("Select action", actionOptions) + 1;


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
                    var users = await userService.GetUsers();
                    var roles = await permissionService.GetRoles();

                    var selectedUserIndex = KeyboardNavigation.GetMenuChoice(
                        "Select user",
                        users.Select(u => u.Email).ToArray()
                    );
                    var selectedUser = users[selectedUserIndex];

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
                    await seedService.SeedCatBreeds();

                    /*
                    await seedService.SeedCats(true, 50);
                    await seedService.SeedCatRelations();
                    await seedService.SeedCatPhotos();
                   */
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
                                                       confirmOptions);

                        if (confirmOptions[selectedOptionIndex] != "Yes")
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
