using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace DeliveryTrackingApp.Controllers;

using Constants;
using Helpers;
using Hubs;
using Models;

public class UserController : Controller
{
    private readonly ILogger<UserController> _logger;
    private readonly DeliveryDbContext _db;
    private readonly IHubContext<DriverStatusHub> _hubContext;

    public UserController(ILogger<UserController> logger, DeliveryDbContext db, IHubContext<DriverStatusHub> hubContext)
    {
        _logger = logger;
        _db = db;
        _hubContext = hubContext;
    }

    // Check thêm role user thường không thể thấy admin
    [Authorize(Roles = "Admin")]
    [HttpGet]
    public IActionResult UserPage(string? keyword)
    {
        var query = _db.Users.Where(p => !p.IsDeleted);

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
        var stopwatch = Stopwatch.StartNew(); // bắt đầu đo

        var trips = _db.DeliveryTrips.Where(p => p.UserId == id && !p.IsDeleted)
            .OrderByDescending(p => p.CreatedOn)
            .Include(p => p.DeliveryNote)
            .ToList()
            .Select(p =>
            {
                var dto = p.ToSearchDto();
                var deliveryTime = dto.CreatedOn; // thời điểm giao hàng thực tế
                var requiredTime = p.DeliveryNote?.DeliveryTime ?? DateTime.MaxValue;

                // 1. Lấy cảnh báo hết tài xế trước thời điểm giao
                var alert = _db.DriverAlerts
                    .Where(a => a.CreatedOn <= deliveryTime)
                    .OrderByDescending(a => a.CreatedOn)
                    .FirstOrDefault();

                var deadline = requiredTime;

                var deliveryDate = requiredTime.Date;
                var isToday = deliveryDate == DateTime.Today;
                var now = DateTime.Now;

                // 2. Nếu hôm nay và sau 17h45 mà chưa giao → dời deadline sang 8h hôm sau
                if (isToday && now.TimeOfDay > Setting.TimeOff.ToTimeSpan() && deliveryTime > requiredTime)
                {
                    var nextDay = deliveryDate.AddDays(1);
                    deadline = nextDay + Setting.TimeToWork.ToTimeSpan(); // 08:00 ngày mai
                }
                // 3. Nếu có cảnh báo hết tài xế trước thời hạn
                else if (alert?.CreatedOn < requiredTime)
                {
                    deadline = alert.CreatedOn;
                }
                // 4. Nếu không rơi vào 2 trường hợp trên → tìm lúc tài xế bắt đầu rảnh
                else
                {
                    var readyTime = _db.DeliveryTrips
                        .Where(t => t.UserId == p.UserId && t.TripType == 1 && !t.IsDeleted && t.CreatedOn <= deliveryTime)
                        .OrderByDescending(t => t.CreatedOn)
                        .Select(t => (DateTime?)t.CreatedOn)
                        .FirstOrDefault();

                    if (readyTime.HasValue)
                    {
                        deadline = readyTime.Value;
                    }
                }

                // 5. So sánh deadline với thời điểm giao thực tế
                dto.Status = deliveryTime >= deadline.AddMinutes(Setting.LateDelivery) ? "Giao trễ" : "Đúng giờ";

                return dto;
            })
            .ToList();

        stopwatch.Stop(); // dừng đồng hồ

        var elapsedMilliseconds = stopwatch.ElapsedMilliseconds;

        // Bạn có thể log ra console, file, hoặc hiển thị tạm thời
        Console.WriteLine($"[DEBUG] GetUserTrips mất {elapsedMilliseconds}ms cho UserId: {id}");

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
                    role = Role.Create(RoleString.Driver, "AD01");
                    _db.Roles.Add(role);
                    _db.SaveChanges();
                }

                var newUser = Models.User.Create(userId, model.Username + "", model.FullName + "", role.Id, "AD01");
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
}
