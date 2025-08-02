using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DeliveryTrackingApp.Controllers;

using Models;
using static Models.Role;

[Authorize(Roles = "Admin")]
public class RoleController : Controller
{
    private readonly ILogger<RoleController> _logger;
    private readonly DeliveryDbContext _db;

    public RoleController(ILogger<RoleController> logger, DeliveryDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet]
    public IActionResult RolePage(string? keyword)
    {
        var query = _db.Roles.Where(p => !p.IsDeleted);

        var roles = query.Select(p => p.ToSearchDto()).ToList();

        return View(roles);
    }

    // POST: /Role/Delete/5
    [HttpPost]
    public IActionResult Delete(Guid id)
    {
        var role = _db.Roles.FirstOrDefault(p => p.Id == id && p.IsDeleted == false);
        if (role != null)
        {
            var createBy = User.FindFirst("UserId")?.Value;
            role.Delete(createBy);//Replace to AdminId
            _db.SaveChanges();
            TempData["Message"] = "Xóa quyền thành công";
        }
        else
        {
            TempData["Error"] = "Không tìm thấy quyền này";
        }
        return RedirectToAction("RolePage");
    }

    // GET: /Role/Create
    public IActionResult RoleCreate()
    {
        return View();
    }

    // POST: /Role/RoleCreate
    [HttpPost]
    public IActionResult RoleCreate(Models.Role.ViewDto model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var createBy = User.FindFirst("UserId")?.Value;
                var newRole = Models.Role.Create(model.Name, createBy);
                _db.Roles.Add(newRole);
                _db.SaveChanges();
                return RedirectToAction("RolePage");
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo quyền.");
            }
        }
        return View(model);
    }

    // GET: /Role/RoleUpdate/Id
    public IActionResult RoleUpdate(Guid id)
    {
        var role = _db.Roles.Find(id);
        if (role == null) return NotFound();
        return View(role.ToViewDto());
    }

    // POST: /Role/RoleUpdate/5
    [HttpPost]
    public IActionResult RoleUpdate(RoleUpdateDto model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var createBy = User.FindFirst("UserId")?.Value;
                var role = _db.Roles.Find(model.Id);
                if (role == null) return NotFound();

                role.Update(model.Name, createBy);
                _db.Roles.Update(role);
                _db.SaveChanges();

            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi update quyền.");
            }
        }
        return RedirectToAction("RolePage");
    }

}
