using Azure.Identity;
using Azure.Storage.Blobs;
using Kissarekisteri.Authorization;
using Kissarekisteri.Database;
using Kissarekisteri.Filters;
using Kissarekisteri.RBAC;
using Kissarekisteri.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IO;
using System.Reflection;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .WithOrigins("https://localhost:5173", "kissarekisteri.b2clogin.com")
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
        options.AddPolicy(
            permission.ToString(),
            policy => policy.Requirements.Add(new PermissionRequirement(permission))
        );
    }
});

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority =
            "https://kissarekisteri.b2clogin.com/kissarekisteri.onmicrosoft.com/B2C_1_SIGN_IN_SIGN_UP/v2.0/";
        options.Audience = "8f374d27-54ee-40d1-bed8-ba2f8a4bd1f6";

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidAudience = "8f374d27-54ee-40d1-bed8-ba2f8a4bd1f6",
            ValidIssuer =
                "https://kissarekisteri.b2clogin.com/d128e5ef-7125-45c2-8e8c-4fd41c0c862e/v2.0/"
        };
    });

builder.Services.AddDbContext<KissarekisteriDbContext>(
    options => options.UseSqlServer(builder.Configuration.GetConnectionString("developmentSQL"))
);

builder.Services
    .AddControllers(options => options.Filters.Add(new ModelValidationFilter()))
    .AddJsonOptions(
        options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles
    );

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(options =>
{
    options.AddJWTAuth();
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

app.UseExceptionHandler(appBuilder =>
{
    appBuilder.Run(async context =>
    {
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        var exceptionHandlerFeature = context.Features.Get<IExceptionHandlerFeature>();
        if (exceptionHandlerFeature != null)
        {
            var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(exceptionHandlerFeature.Error, "An unhandled exception has occurred.");

            await context.Response.WriteAsync("Internal Server Error");
        }
    });
});

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<KissarekisteriDbContext>();
    var seedService = scope.ServiceProvider.GetRequiredService<SeedService>();

    dbContext.Database.EnsureCreated();

    /*
    await seedService.SeedCatBreeds();
    await seedService.SeedPermissions();
    await seedService.SeedRoles();
    await seedService.SeedRolePermissions();
    await seedService.UpdateUserRoles();
    */

}

app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.Run();
