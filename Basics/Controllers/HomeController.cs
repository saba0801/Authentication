using Basics.CustomPolicyProvider;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Basics.Controllers
{
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }
        [Authorize]
        public IActionResult Secret()
        {
            return View();
        }
        [Authorize(Policy = "Claim.DoB")]
        public IActionResult SecretPolicy()
        {
            return View("Secret");
        }
        [Authorize(Roles = "saba")]
        public IActionResult SecretRole()
        {
            return View("Secret");
        }
        [SecurityLevel(5)]
        public IActionResult SecretLevel()
        {
            return View("Secret");
        }
        [SecurityLevel(10)]
        public IActionResult SecretHigherLevel()
        {
            return View("Secret");
        }
        [AllowAnonymous]
        public IActionResult Authenticate()
        {
            var grandmaClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,"Bob"),
                new Claim(ClaimTypes.Email,"Bob@fmail.com"),
                new Claim(ClaimTypes.DateOfBirth,"11/11/2000"),
                new Claim(ClaimTypes.Role,"saba"),
                new Claim(DynamicPolicies.SecurityLevel,"7"),
                new Claim("Grandma.Says","Very nice boi."),  
            };
            var licenseClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name,"Bob K Foo"),
                new Claim("Driving License", "A+"),
            };
            var grandmaIdentity = new ClaimsIdentity(grandmaClaims, "Grandma Identity");
            var licenseIdentity = new ClaimsIdentity(licenseClaims, "Government");
            var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity,licenseIdentity });
            //-------------------------------------------------------------
            HttpContext.SignInAsync(userPrincipal); 
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Dostuff(
            [FromServices] IAuthorizationService authorizationService)
        {
        //    private readonly IAuthorizationService _authorizationService;

        //public HomeController(IAuthorizationService authorizationService)
        //{
        //    _authorizationService = authorizationService;

        //}
        //We are doing stuff here
        var builder = new AuthorizationPolicyBuilder("Schema");
            var customPolicy = builder.RequireClaim("Hello").Build();

           var authResult = await authorizationService.AuthorizeAsync(User, customPolicy);
            if(authResult.Succeeded)
            {
                return View("Index");
            }

            return View("Index");
        }
    }
}
