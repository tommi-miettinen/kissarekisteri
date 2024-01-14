using Azure.Identity;
using Azure.Storage.Blobs;
using Kissarekisteri.Database;
using Kissarekisteri.Filters;
using Kissarekisteri.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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
var instance = config["AzureAdB2C:Instance"];
var domain = config["AzureAdB2C:Domain"];
var policyName = config["AzureAdB2C:SignUpSignInPolicyId"];
var appId = config["AzureAdB2C:ClientId"];
var clientSecret = config["AzureAdB2C:ClientSecret"];
var tenantId = config["AzureAdB2C:TenantId"];
var dbConnectionString = builder.Environment.IsDevelopment()
          ? config.GetConnectionString("developmentSQL")
          : config.GetConnectionString("AzureSQL");

var devFrontendUrl = "https://localhost:5173";


builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder
            .WithOrigins(devFrontendUrl)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddSingleton(serviceProvider =>
{
    var clientSecretCredential = new ClientSecretCredential(domain, appId, clientSecret);
    var scopes = new[] { "https://graph.microsoft.com/.default" };
    return new GraphServiceClient(clientSecretCredential, scopes);
});

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


builder.Services.AddDbContext<KissarekisteriDbContext>(options => options.UseSqlServer(dbConnectionString));

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
