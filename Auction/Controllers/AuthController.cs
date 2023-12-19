using Auction.Models.Auth;
using Auction.Models.ConstModels;
using Auction.Models.MSSQLModels;
using Auction.Models.MSSQLModels.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Auction.Controllers
{
    public class AuthController(LocalDBContext context) : Controller
    {
        private readonly LocalDBContext _context = context;
        private readonly int keySize = 64;
        private readonly int iterations = 350000;
        private readonly HashAlgorithmName hashAlgorithm = HashAlgorithmName.SHA512;

        private string HashPasword(string password, out byte[] salt)
        {
            salt = RandomNumberGenerator.GetBytes(keySize);
            var hash = Rfc2898DeriveBytes.Pbkdf2(
                Encoding.UTF8.GetBytes(password),
                salt,
                iterations,
                hashAlgorithm,
                keySize);
            return Convert.ToHexString(hash);
        }

        private bool VerifyPassword(string password, string hash, byte[] salt)
        {
            var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, hashAlgorithm, keySize);
            return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
        }

        private string SetCoockieValue(string key, string value)
        {
            if(HttpContext.Request.Cookies.ContainsKey(key))
                HttpContext.Response.Cookies.Delete(key);
            HttpContext.Response.Cookies.Append(key, value);
            return value;
        }

        private string? GetCoockieValue(string key)
        {//Reset coockie
            string? v = null;
            bool valueExist = HttpContext.Request.Cookies.ContainsKey(key) && HttpContext.Request.Cookies.TryGetValue(key, out v) && v is not null;
            if (key == CoockieEnums.ThemeObject.Key)
            {
                if (valueExist && CoockieEnums.ThemeObject.Values.Contains(v!))
                    return v;
                return SetCoockieValue(key, CoockieEnums.ThemeObject.Default);
            }
            else
            {
                return null;
            }
        }

        private async Task AuthorizeAsync(User user, bool RememberMe, string? returnUrl)
        {
            var claims = new List<Claim>
                    {
                        new(ClaimTypes.Sid, user.Id.ToString()),
                        new(ClaimTypes.Name, user.Username),
                        new(ClaimTypes.Role, user.Role.Name),
                    };
            ClaimsIdentity id = new(
                claims,
                CookieAuthenticationDefaults.AuthenticationScheme,
                ClaimTypes.Name,
                ClaimTypes.Role
            );
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id), new()
            {
                AllowRefresh = true,
                RedirectUri = returnUrl,
                ExpiresUtc = DateTime.UtcNow.AddMinutes(1),
                IssuedUtc = DateTime.UtcNow,
                IsPersistent = RememberMe,
            });
        }



        [HttpGet]
        public IActionResult Register()
        {
            ViewData["Theme"] = GetCoockieValue(CoockieEnums.ThemeObject.Key);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            [Bind($"{nameof(RegisterVM.Username)},{nameof(RegisterVM.Password)},{nameof(RegisterVM.RepeatPassword)}")] RegisterVM register, 
            string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                if(!_context.Users.Any(u => u.Username == register.Username))
                {
                    User user = new()
                    {
                        Id = Guid.NewGuid(),
                        Username = register.Username,
                        Hash = HashPasword(register.Password, out byte[] salt),
                        Salt = Convert.ToHexString(salt),
                        RoleId = 1
                    };
                    _context.Add(user);
                    await _context.SaveChangesAsync();
                    _context.Roles.ToList();
                    await AuthorizeAsync(user, false, returnUrl);
                    return Redirect(returnUrl ?? "/");
                }
                ModelState.AddModelError("", "Имя пользователя уже занято!");
            }
            register.Password = null!;
            register.RepeatPassword = null!;
            ViewData["Theme"] = GetCoockieValue(CoockieEnums.ThemeObject.Key);
            return View(register);
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewData["Theme"] = GetCoockieValue(CoockieEnums.ThemeObject.Key);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            [Bind($"{nameof(LoginVM.Username)},{nameof(LoginVM.Password)},{nameof(LoginVM.RememberMe)}")] LoginVM login, 
            string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                User? user = _context.Users.Where(u => u.Username == login.Username).Include(r => r.Role).FirstOrDefault();
                if (user is not null && VerifyPassword(login.Password, user.Hash, Convert.FromHexString(user.Salt)))
                {
                    await AuthorizeAsync(user, login.RememberMe, returnUrl);
                    return Redirect(returnUrl ?? "/");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            login.Password = null!;
            login.RememberMe = false;
            ViewData["Theme"] = GetCoockieValue(CoockieEnums.ThemeObject.Key);
            return View(login);
        }

        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
