using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryTrackingApp.Controllers;

using Constants;
using Helpers;
using Models;
using Requests;

public class AuthenticationController : Controller
{
    private readonly ILogger<AuthenticationController> _logger;
    private readonly DeliveryDbContext _db;

    public AuthenticationController(ILogger<AuthenticationController> logger, DeliveryDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginR request)
    {
        var user = ValidateLogin(request); // Lấy User từ DB

        if (user != null)
        {
            // Truy vấn RoleName từ RoleId
            var role = _db.Roles.FirstOrDefault(r => r.Id == user.RoleId);
            var roleName = role?.Name ?? RoleString.Driver;

            // Tạo Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim("UserId", user.Id),
                new Claim("FullName", user.FullName ?? ""),
                new Claim(ClaimTypes.Role, roleName)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Tạo cookie xác thực
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            TempData[Success.Login] = Message.Login;
            return RedirectToAction("UserPage", "User");
        }
        else
        {
            ModelState.AddModelError("Error", "Tài khoản hoặc mật khẩu không chính xác, vui lòng thử lại");
            return View("Login");
        }
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public ActionResult Register(RegisterR request)
    {
        if (ModelState.IsValid)
        {
            var ett = _db.Users.FirstOrDefault(p => p.Username == request.Username);
            var role = _db.Roles.FirstOrDefault(p => p.Name.ToLower() == RoleString.Driver.ToLower());

            if (role == null)
            {
                role = Role.Create(RoleString.Driver, "System");
                _db.Roles.Add(role);
                _db.SaveChanges();
            }

            if (ett != null)
            {
                ModelState.AddModelError("Username", "Username đã tồn tại ! Vui lòng sử dụng 1 email khác");
                return View();
            }
            if (request.Password != request.RePassword)
            {
                ModelState.AddModelError("Password", "Mật khẩu không khớp vui lòng thử lại");
            }
            else
            {
                string hashedPassword = PasswordHash.HashPassword(request.Password);
                ett = Models.User.Create(request.Username + "", hashedPassword, request.FullName + "", role.Id, "");

                _db.Users.Add(ett);
                _db.SaveChanges();
                return RedirectToAction("Login", "Authentication");
            }
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        // Đăng xuất khỏi hệ thống
        await HttpContext.SignOutAsync();

        // Chuyển về trang đăng nhập
        return RedirectToAction("Login", "Authentication");
    }

    private User.SearchDto? ValidateLogin(LoginR request)
    {
        if (request.Password == null || request.Username == null)
        {
            return null;
        }

        string hashedPassword = PasswordHash.HashPassword(request.Password);

        var ett = _db.Users
            .Where(p => (p.Username == request.Username)
            && (p.PasswordHash == hashedPassword || p.PasswordHash == request.Password) && p.IsDeleted == false)
            .Select(p => p.ToSearchDto()).FirstOrDefault();

        return ett;
    }
}
