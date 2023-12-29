using Azure.Identity;
using Azure.Storage.Blobs;
using Kissarekisteri.Authorization;
using Kissarekisteri.Database;
using Kissarekisteri.Filters;
using Kissarekisteri.RBAC;
using Kissarekisteri.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;
using Microsoft.Identity.Web;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .WithOrigins("https://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
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

builder.Services.AddSingleton(new BlobServiceClient(config.GetConnectionString("Storage")));

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UploadService>();
builder.Services.AddScoped<CatService>();
builder.Services.AddScoped<CatShowService>();
builder.Services.AddScoped<PermissionService>();
builder.Services.AddScoped<SeedService>();
builder.Services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

builder.Services.AddAuthorization(options =>
{


    foreach (PermissionType permission in Enum.GetValues(typeof(PermissionType)))
    {
        options.AddPolicy(permission.ToString(), policy =>
            policy.Requirements.Add(new PermissionRequirement(permission)));
    }
});


builder.Services.AddMicrosoftIdentityWebAppAuthentication(
    config,
    Microsoft.Identity.Web.Constants.AzureAdB2C
);

builder.Services.Configure<OpenIdConnectOptions>(
    OpenIdConnectDefaults.AuthenticationScheme,
    options =>
    {
        options.Scope.Add(options.ClientId);
        options.Events = new OpenIdConnectEvents
        {
            OnTokenValidated = async context =>
            {
                await context.HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    context.Principal
                );
                context.Response.Redirect("https://localhost:5173/cats");
                context.HandleResponse();
            },
        };
    }
);

builder.Services.Configure<CookieAuthenticationOptions>(
       CookieAuthenticationDefaults.AuthenticationScheme,
          options =>
          {
              options.Events = new CookieAuthenticationEvents
              {
                  OnRedirectToLogin =
                   context =>
                  {
                      context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                      return Task.CompletedTask;
                  },
                  OnRedirectToAccessDenied = context =>
                  {
                      context.Response.StatusCode = StatusCodes.Status403Forbidden;
                      return Task.CompletedTask;
                  },
              };
          });

builder.Services.AddDbContext<KissarekisteriDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DevelopmentSQL"));
});

builder.Services
    .AddControllers(options =>
    {
        options.Filters.Add(new ModelValidationFilter());
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{

    app.UseDeveloperExceptionPage();
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<KissarekisteriDbContext>();

        /*  
           context.Database.EnsureDeleted();
           context.Database.EnsureCreated();
           context.SeedCatBreeds();
           context.SeedPermissions();
           context.SeedRoles();
           context.SeedRolePermissions();
        */
    }
}

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SupportedSubmitMethods([]);
});
app.UseCors();
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
