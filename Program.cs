using Azure.Identity;
using Azure.Storage.Blobs;
using Kissarekisteri.Database;
using Kissarekisteri.DTOs;
using Kissarekisteri.Filters;
using Kissarekisteri.Models;
using Kissarekisteri.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Graph;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OData.ModelBuilder;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;

var instance = config["AzureAdB2C:Instance"];
var domain = config["AzureAdB2C:Domain"];
var policyName = config["AzureAdB2C:SignUpSignInPolicyId"];
var appId = config["AzureAdB2C:ClientId"];
var tenantId = config["AzureAdB2C:TenantId"];
var clientSecret = config["AzureAdB2C:ClientSecret"];

var productionSql = config.GetConnectionString("AzureSQL");
var developmentSql = config.GetConnectionString("developmentSQL");
var dbConnectionString = builder.Environment.IsDevelopment() ? developmentSql : productionSql;

var devFrontendUrl = "https://localhost:5173";

var configMap = new Dictionary<string, string>
{
    [nameof(instance)] = instance,
    [nameof(domain)] = domain,
    [nameof(policyName)] = policyName,
    [nameof(appId)] = appId,
    [nameof(clientSecret)] = clientSecret,
    [nameof(tenantId)] = tenantId,
    [nameof(devFrontendUrl)] = devFrontendUrl,
    [nameof(dbConnectionString)] = dbConnectionString
};

foreach (var value in configMap)
{
    if (string.IsNullOrEmpty(value.Value))
    {
        throw new Exception($"Missing value for {value.Key}");
    }
}

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(builder =>
    {
        builder
            .WithOrigins(devFrontendUrl)
            .AllowAnyHeader()
            .AllowAnyMethod();
    }));

var credential = new ClientSecretCredential(domain, appId, clientSecret);
builder.Services.AddSingleton(new GraphServiceClient(credential, ["https://graph.microsoft.com/.default"]));
builder.Services.AddSingleton(new BlobServiceClient(config.GetConnectionString("Storage")));
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UploadService>();
builder.Services.AddScoped<CatService>();
builder.Services.AddScoped<CatShowService>();
builder.Services.AddScoped<PermissionService>();

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = $"{instance}/{domain}/{policyName}/v2.0/";
        options.Audience = appId;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidAudience = appId,
            ValidIssuer = $"{instance}/{tenantId}/v2.0/"
        };
    });

builder.Services.AddDbContext<KissarekisteriDbContext>(options => options.UseSqlServer(config.GetConnectionString("AzureSQL")), ServiceLifetime.Scoped);

var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EnableLowerCamelCase();
modelBuilder.EntitySet<MsalConfigDTO>("MsalConfig").EntityType.HasKey(dto => dto.Id);
modelBuilder.EntitySet<Cat>("Cats");
modelBuilder.EntitySet<CatBreed>("CatBreeds");
modelBuilder.EntitySet<CatShow>("CatShows");
modelBuilder.EntitySet<Role>("Roles");
modelBuilder.EntitySet<UserResponse>("Users");

builder.Services
    .AddControllers(options => options.Filters.Add(new ModelValidationFilter()))
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false));
    }).AddOData(
    options => options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(null).AddRouteComponents(
        "odata",
        modelBuilder.GetEdmModel()));


builder.Services.AddSwaggerGen(options =>
{
    options.AddJWTAuth();
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

app.UseExceptionHandler(appBuilder =>
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
    }));

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
