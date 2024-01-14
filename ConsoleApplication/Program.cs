
using Azure.Identity;
using Azure.Storage.Blobs;
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
    static async Task Main()
    {
        Assembly assembly = Assembly.Load("Kissarekisteri");

        var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddUserSecrets(assembly)
                .Build();

        var env = "Development";

        var instance = config["AzureAdB2C:Instance"];
        var domain = config["AzureAdB2C:Domain"];
        var policyName = config["AzureAdB2C:SignUpSignInPolicyId"];
        var appId = config["AzureAdB2C:ClientId"];
        var clientSecret = config["AzureAdB2C:ClientSecret"];
        var tenantId = config["AzureAdB2C:TenantId"];
        var dbConnectionString = env == "Development"
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
            Console.WriteLine("Choose an action:");
            Console.WriteLine("1. Create user");
            Console.WriteLine("2. Assign Role to User");
            Console.WriteLine("Press any key to exit...");


            var userInput = Console.ReadLine();

            var jsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                ReferenceHandler = ReferenceHandler.IgnoreCycles
            };

            string Json(object obj) => JsonSerializer.Serialize(obj, jsonSerializerOptions);

            switch (userInput)
            {
                case "1":
                    Console.WriteLine("Enter email:");
                    var email = Console.ReadLine();
                    var password = "";

                    if (email != null)
                    {
                        Console.WriteLine("Password:");
                        password = Console.ReadLine();

                        if (password != null)
                        {
                            var user4 = new { email, password };
                            Console.WriteLine("Create user (Y/N)?");

                            var roles4 = await permissionService.GetRoles();
                            Console.WriteLine(Json(roles4));
                        }
                    }
                    break;

                case "2":
                    var userss = await userService.GetUsers();
                    var roles = await permissionService.GetRoles();

                    Console.WriteLine("USERS");
                    Console.WriteLine(Json(userss));

                    Console.WriteLine("Enter user id:");
                    var userId5 = Console.ReadLine();
                    var user5 = await userService.GetUserById(userId5);

                    Console.WriteLine("ROLES");
                    Console.WriteLine(Json(roles));

                    Console.WriteLine("Enter role id:");
                    var roleId5 = Console.ReadLine();

                    var role5 = await permissionService.GetRoleById(int.Parse(roleId5));

                    if (userId5 == null || roleId5 == null)
                    {
                        Console.WriteLine("Invalid input. Please choose a valid action.");
                        break;
                    }

                    Console.WriteLine("Assign " + Json(role5) + " to " + Json(user5) + "? (Y/N)");

                    var confirmation = Console.ReadLine();

                    if (confirmation == "Y" || confirmation == "y")
                    {
                        await permissionService.AssignRole(userId5, int.Parse(roleId5));
                        Console.WriteLine("Role assigned.");
                        break;
                    }
                    break;


                default:
                    Console.WriteLine("Invalid input. Please choose a valid action.");
                    break;
            }
        }
    }
}