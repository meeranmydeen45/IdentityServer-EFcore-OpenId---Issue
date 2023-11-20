using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;

namespace Client.Mvc.Controllers
{
    [Route("[Controller]")]
    public class HomeController : Controller
    {
        private readonly HttpContext? contex;
        public HomeController(IHttpContextAccessor accessor)
        {
            contex = accessor.HttpContext;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet("secret")]
        public async Task<IActionResult> Secret()
        {
            var accessToken = await contex.GetTokenAsync("access_token");
            var idToken = await contex.GetTokenAsync("id_token");
            var refreshToken = await contex.GetTokenAsync("refresh_token");

            var claims = User.Claims;
            var  _accessToken = new JwtSecurityTokenHandler().ReadJwtToken(accessToken);
            var _idToken = new JwtSecurityTokenHandler().ReadJwtToken(idToken);

            return View();
        }
    }
}
