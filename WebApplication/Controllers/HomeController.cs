using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Claims;
using WebApplication.Models;

namespace WebApplication.Controllers {
    public class HomeController : Controller {
        private readonly ILogger<HomeController> _logger;
        private IHttpContextAccessor _httpContextAccessor;
        private readonly IOptionsManager<DemoOptions> _options;

        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor, IOptionsManager<DemoOptions> options) {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _options = options;
        }

        public IActionResult Index() {
            return View();
        }

        [HttpPost]
        public async System.Threading.Tasks.Task<ActionResult> IndexAsync(int tenantId) {
            var claims = new List<Claim> { new Claim("TenantId", tenantId.ToString()) };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties {

            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            //var identity = new ClaimsIdentity();
            //identity.AddClaim(new Claim("TenantId", tenantId.ToString()));
            //_httpContextAccessor.HttpContext.User.AddIdentity(identity);

            return View();
        }

        public IActionResult OptionInfo() {
            var values = _options.Value;
            return View(values);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
