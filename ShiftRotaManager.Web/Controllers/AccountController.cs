using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

public class AccountController : Controller
{
    private const string TestUserName = "Test123";
    private const string TestPassword = "Pa$$word123";

    [HttpGet]
    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string userName, string password)
    {
        if (userName == TestUserName && password == TestPassword)
        {
            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, userName)
                 };
            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
            return RedirectToAction("Index", "Dashboard");
        }
        ViewBag.Error = "Invalid username or password.";
        return View();
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}