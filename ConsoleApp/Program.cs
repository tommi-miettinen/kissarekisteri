
using Azure.Identity;
using Azure.Storage.Blobs;
using Kissarekisteri.Database;
using Kissarekisteri.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System;
using System.Text.Json;
using System.Threading.Tasks;


namespace KissarekisteriConsole.ConsoleApp;

public class Program
{
    static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
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
        var catShowService = new CatShowService(dbContext, userService, uploadService);
        var seedService = new SeedService(userService, catService, catShowService, dbContext);



        while (true)
        {
            Console.WriteLine("Choose an action:");
            Console.WriteLine("1. Get Cats");
            Console.WriteLine("2. Get Cat Shows");
            Console.WriteLine("3. Get Users");
            Console.WriteLine("4. Exit");

            var userInput = Console.ReadLine();

            switch (userInput)
            {
                case "1":
                    var cats = await catService.GetCatsAsync();

                    var jsonCats = JsonSerializer.Serialize(cats, new JsonSerializerOptions
                    {
                        WriteIndented = true // Format the JSON for readability
                    });
                    Console.WriteLine(jsonCats);
                    break;

                case "2":
                    var catShows = await catShowService.GetCatShows();
                    // Serialize and print JSON data
                    var jsonCatShows = JsonSerializer.Serialize(catShows, new JsonSerializerOptions
                    {
                        WriteIndented = true // Format the JSON for readability
                    });
                    Console.WriteLine(jsonCatShows);
                    break;

                case "3":
                    var users = await userService.GetUsers();
                    // Serialize and print JSON data
                    var jsonUsers = JsonSerializer.Serialize(users, new JsonSerializerOptions
                    {
                        WriteIndented = true // Format the JSON for readability
                    });
                    Console.WriteLine(jsonUsers);
                    break;

                case "4":
                    return; // Exit the program

                default:
                    Console.WriteLine("Invalid input. Please choose a valid action.");
                    break;
            }
        }
    }
}