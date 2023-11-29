using Kissarekisteribackend.Database;
using Kissarekisteribackend.Models;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using System.Linq;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.Unspecified;
    options.HandleSameSiteCookieCompatibility();
});


builder.Services.AddMicrosoftIdentityWebAppAuthentication(config, Constants.AzureAdB2C);


builder.Services.Configure<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme, options =>
{
    options.Scope.Add(options.ClientId);

    options.Events = new OpenIdConnectEvents
    {
        OnTokenValidated = async context =>
        {
            var userId = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;


            if (userId != null)
            {
                using (var scope = context.HttpContext.RequestServices.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<KissarekisteriDbContext>();

                    var user = dbContext.Users.FirstOrDefault(u => u.Id == userId);
                    if (user == null)
                    {
                        user = new User { Id = userId, Username = userId };
                        dbContext.Users.Add(user);
                        await dbContext.SaveChangesAsync();
                    }
                }
            }
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
}

app.UseHttpsRedirection();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
