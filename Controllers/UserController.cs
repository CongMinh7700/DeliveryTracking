using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryTrackingApp.Controllers;

using Constants;
using Helpers;
using Hubs;
using Models;
using Services.Interface;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly DeliveryDbContext _db;
    private readonly IHubContext<DriverStatusHub> _hubContext;
    private readonly ICurrentUserService _currentUser;

    public UserController(ILogger<UserController> logger, DeliveryDbContext db, IHubContext<DriverStatusHub> hubContext, ICurrentUserService currentUser)
    {
        _logger = logger;
        _db = db;
        _hubContext = hubContext;
        _currentUser = currentUser;
    }

    // Check thêm role user thường không thể thấy admin
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult UserPage(string? keyword)
    {
        var query = _db.Users.Where(u => !u.IsDeleted && u.Role != null && u.Role.Name != RoleString.Admin);

        if (!string.IsNullOrEmpty(keyword))
        {
            query = query.Where(u => u.Username.ToLower().Contains(keyword.ToLower()));
        }

        var users = query.Select(p => p.ToSearchDto()).ToList();

        foreach (var user in users)
        {
            var latestTrip = _db.DeliveryTrips
                .Where(d => d.UserId == user.Id && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedOn)
                .FirstOrDefault();

            user.Status = latestTrip?.TripType == 0 ? DriverStatus.Busy : DriverStatus.Available;

        }
        return View(users);
    }


    // UserDetail/id
    [HttpGet]
    public IActionResult UserDetail(string id)
    {
        var query = _db.Users.Where(p => !p.IsDeleted && p.Id == id).Include(p => p.DeliveryTrips);

        var user = query.Select(p => p.ToViewDto()).FirstOrDefault();
        if (user != null)
        {
            var latestTrip = _db.DeliveryTrips
                .Where(d => d.UserId == user.Id && !d.IsDeleted)
                .OrderByDescending(d => d.CreatedOn)
                .FirstOrDefault();

            user.Status = latestTrip?.TripType == 0 ? DriverStatus.Busy : DriverStatus.Available;
        }

        return View(user);
    }

    [HttpGet("api/users/{id}/trips")]
    public IActionResult GetUserTrips(string id)
    {
        var trips = _db.DeliveryTrips.Where(p => p.UserId == id && !p.IsDeleted)
            .Include(p => p.DeliveryNote)
            .OrderByDescending(p => p.CreatedOn)
            .ToList()
            .Select(p => p.ToSearchDto()).ToList();

        return Ok(trips);
    }

    // POST: /User/Delete/5
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(string id)
    {
        var user = _db.Users.FirstOrDefault(p => p.Id == id && p.IsDeleted == false);
        if (user != null)
        {
            user.Delete(_currentUser.UserId);
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
    [Authorize(Roles = "Admin")]
    public IActionResult UserCreate()
    {
        return View();
    }

    // POST: /User/UserCreate
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult UserCreate(Models.User.ViewDto model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var userId = GenerateStringId.GenerateCode(_db.Users, u => u.Id, "GN", 2);

                var role = _db.Roles.FirstOrDefault(p => p.Name.ToLower() == RoleString.Driver.ToLower());
                if (role == null)
                {
                    role = Role.Create(RoleString.Driver, _currentUser.UserId);
                    _db.Roles.Add(role);
                    _db.SaveChanges();
                }

                var newUser = Models.User.Create(userId, model.Username + "", model.FullName + "", role.Id, _currentUser.UserId);
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
    public IActionResult UserUpdate(User.UserUpdateDto model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var user = _db.Users.Find(model.Id);
                if (user == null) return NotFound();

                user.Update(model.Username, model.FullName, _currentUser.UserId);
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
}
