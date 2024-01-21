using Kissarekisteri.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;


namespace Kissarekisteri.Controllers
{
    public class ConfigController(IConfiguration config, IHostEnvironment env) : ODataController
    {
        [HttpGet("odata/msalconfig")]
        [EnableQuery]
        public ActionResult Get()
        {
            var request = HttpContext.Request;
            var adb2cConfig = config.GetSection(Microsoft.Identity.Web.Constants.AzureAdB2C);
            var currentUrl = $"https://{request.Host}{request.PathBase}";
            var developmentUrl = "https://localhost:5173";

            var msalConfig = new MsalConfigDTO
            {
                Id = 1,
                AuthorityDomain = "kissarekisteri.b2clogin.com",
                Authority = "https://kissarekisteri.b2clogin.com/kissarekisteri.onmicrosoft.com/b2c_1_sign_in_sign_up",
                ClientId = adb2cConfig["ClientId"],
                Instance = adb2cConfig["Instance"],
                Domain = adb2cConfig["Domain"],
                RedirectUri = env.IsDevelopment() ? developmentUrl : currentUrl
            };

            List<MsalConfigDTO> configList = [msalConfig];

            return Ok(configList);
        }
    }
}
