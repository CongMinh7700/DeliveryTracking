using Microsoft.AspNetCore.Mvc;

namespace DeliveryTrackingApp.Controllers;

using Models;
using static DeliveryTrackingApp.Models.User;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly DeliveryDbContext _db;
    public UserController(ILogger<UserController> logger, DeliveryDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    // Check thêm role user thường không thể thấy admin
    [HttpGet]
    public IActionResult UserPage(string? keyword)
    {
        var query = _db.Users.Where(p => !p.IsDeleted);

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(u => u.Username.ToLower().Contains(keyword.ToLower()));
        }

        var users = query.Select(p => p.ToSearchDto()).ToList();

        return View(users);
    }

    // POST: /User/Delete/5
    [HttpPost]
    public IActionResult Delete(string id)
    {
        var user = _db.Users.FirstOrDefault(p => p.Id == id && p.IsDeleted == false);
        if (user != null)
        {
            user.Delete("AD01"); // Thay 1 bằng UserId thực tế
            _db.SaveChanges();
            TempData["Message"] = "Xóa người dùng thành công";
        }
        else
        {
            TempData["Error"] = "Không tìm thấy người dùng này";
        }
        return RedirectToAction("UserPage");
    }

    // GET: /User/Create
    public IActionResult UserCreate()
    {
        return View();
    }

    // POST: /User/UserCreate
    [HttpPost]
    public IActionResult UserCreate(Models.User.ViewDto model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                string userId = GenerateUserId();
                // Replace Admin =  userID of Admin
                var newUser = Models.User.Create(userId, model.Username + "", model.FullName + "", "Admin");
                _db.Users.Add(newUser);
                _db.SaveChanges();
                return RedirectToAction("UserPage");
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo người dùng.");
            }
        }
        return View(model);
    }

    // GET: /User/UserUpdate/Id
    public IActionResult UserUpdate(string id)
    {
        var user = _db.Users.Find(id);
        if (user == null) return NotFound();
        return View(user.ToViewDto());
    }

    // POST: /User/UserUpdate/5
    [HttpPost]
    public IActionResult UserUpdate(UserUdateDto model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var user = _db.Users.Find(model.Id);
                if (user == null) return NotFound();

                user.Update(model.Username, model.FullName, "AD01");
                _db.Users.Update(user);
                _db.SaveChanges();
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo người dùng.");
            }
        }
        return RedirectToAction("UserPage");
    }

    public string GenerateUserId()
    {
        var lastCode = _db.Users.Where(u => u.Id.StartsWith("GN"))
            .OrderByDescending(u => u.Id)
            .Select(u => u.Id).FirstOrDefault();

        int nextNumber = 1;

        if (!string.IsNullOrEmpty(lastCode) && lastCode.StartsWith("GN"))
        {
            var numberPart = lastCode.Substring(2);
            if (int.TryParse(numberPart, out int current))
            {
                nextNumber = current + 1;
            }
        }

        return $"GN{nextNumber:D2}";
    }
}
