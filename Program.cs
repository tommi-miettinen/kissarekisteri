using Azure.Identity;
using Azure.Storage.Blobs;
using Kissarekisteribackend.Database;
using Kissarekisteribackend.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;
using Microsoft.Identity.Web;


var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.WithOrigins("http://localhost:5173")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});


builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
    options.HandleSameSiteCookieCompatibility();
});

builder.Services.AddSingleton(serviceProvider =>
{

    var graphConfig = config.GetSection("Graph");
    var tenantId = graphConfig["TenantId"];
    var appId = graphConfig["AppId"];
    var clientSecret = graphConfig["ClientSecret"];
    var clientSecretCredential = new ClientSecretCredential(tenantId, appId, clientSecret);

    var scopes = new[] { "https://graph.microsoft.com/.default" };

    return new GraphServiceClient(clientSecretCredential, scopes);
});
builder.Services.AddSingleton(serviceProvider =>
{
    return new BlobServiceClient(config.GetConnectionString("Storage"));
});


builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UploadService>();

builder.Services.AddMicrosoftIdentityWebAppAuthentication(config, Microsoft.Identity.Web.Constants.AzureAdB2C);


builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Scope.Add(options.ClientId);

    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = async context =>
        {
            await context.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, context.Principal);
            context.Response.Redirect("http://localhost:5173/cats");
            context.HandleResponse();
        }
    };
});

builder.Services.AddDbContext<KissarekisteriDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<KissarekisteriDbContext>();
        // context.Database.EnsureDeleted();
        context.Database.EnsureCreated();
    }
}
app.UseCors();
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
