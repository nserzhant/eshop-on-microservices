using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using R2S.Users.AuthorizationServer.Models;
using System.Diagnostics;

namespace R2S.Users.AuthorizationServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IAuthenticationSchemeProvider schemes;

        public HomeController(ILogger<HomeController> logger,
            IAuthenticationSchemeProvider schemes)
        {
            _logger = logger;
            this.schemes = schemes;
        }

        public async Task<IActionResult> Index()
        {
            var defaultScheme = await this.schemes.GetDefaultAuthenticateSchemeAsync();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}