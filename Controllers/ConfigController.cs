using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Kissarekisteri.Controllers
{
    [ApiController]
    [Route("api/config")]
    public class ConfigController(IConfiguration config, IHostEnvironment env) : Controller
    {
        [HttpGet("msalconfig")]
        public ActionResult GetConfig()
        {
            var request = HttpContext.Request;
            var Adb2cConfig = config.GetSection(Microsoft.Identity.Web.Constants.AzureAdB2C);
            var currentUrl = $"https://{request.Host}{request.PathBase}";
            var developmentUrl = "https://localhost:5173";

            return Json(new
            {
                AuthorityDomain = "kissarekisteri.b2clogin.com",
                Authority = "https://kissarekisteri.b2clogin.com/kissarekisteri.onmicrosoft.com/b2c_1_sign_in_sign_up",
                ClientId = Adb2cConfig["ClientId"],
                Instance = Adb2cConfig["Instance"],
                Domain = Adb2cConfig["Domain"],
                redirectUri = env.IsDevelopment() ? developmentUrl : currentUrl,
            });
        }
    }
}
