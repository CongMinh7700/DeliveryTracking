using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DeliveryTrackingApp.Controllers;

using Helpers;
using Models;
using Services.Interface;
using static Models.DeliveryNote;

[Authorize(Roles = "Admin")]
public class DeliveryNoteController : Controller
{
    private readonly ILogger<DeliveryNoteController> _logger;
    private readonly DeliveryDbContext _db;
    private readonly ICurrentUserService _currentUser;
    public DeliveryNoteController(ILogger<DeliveryNoteController> logger, DeliveryDbContext db, ICurrentUserService currentUser)
    {
        _logger = logger;
        _db = db;
        _currentUser = currentUser;
    }

    [HttpGet]
    public IActionResult DeliveryNotePage(string? keyword)
    {
        var query = _db.DeliveryNotes.Where(p => !p.IsDeleted);

        var notes = query.Select(p => p.ToSearchDto()).ToList();

        return View(notes);
    }

    // POST: /DeliveryNote/Delete/5
    [HttpPost]
    public IActionResult Delete(Guid id)
    {
        var note = _db.DeliveryNotes
            .Include(p => p.DeliveryTrips)
            .FirstOrDefault(p => p.Id == id && p.IsDeleted == false);

        if (note != null)
        {
            // Kiểm tra xem có chuyến nào chưa bị xóa không
            bool hasActiveTrips = note.DeliveryTrips.Any(trip => !trip.IsDeleted);

            if (hasActiveTrips)
            {
                TempData["Error"] = "Không thể xóa vì phiếu giao đã có chuyến đi được tạo.";
                return RedirectToAction("DeliveryNotePage");
            }

            ;
            note.Delete(_currentUser.UserId); // Replace bằng AdminId
            _db.SaveChanges();
            TempData["Message"] = "Xóa phiếu giao thành công";
        }
        else
        {
            TempData["Error"] = "Không tìm thấy phiếu giao này";
        }

        return RedirectToAction("DeliveryNotePage");
    }


    // GET: /DeliveryNote/Create
    public IActionResult DeliveryNoteCreate()
    {
        return View();
    }

    // POST: /DeliveryNote/DeliveryNoteCreate
    [HttpPost]
    public IActionResult DeliveryNoteCreate(DeliveryNote.ViewDto model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var code = GenerateStringId.GenerateCode(_db.DeliveryNotes, u => u.Code, "PG", 3);
                var newDeliveryNote = DeliveryNote.Create(code, model.DeliveryTime, model.Note, _currentUser.UserId);
                _db.DeliveryNotes.Add(newDeliveryNote);
                _db.SaveChanges();
                return RedirectToAction("DeliveryNotePage");
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo phiếu giao.");
            }
        }
        return View(model);
    }

    // GET: /DeliveryNote/DeliveryNoteUpdate/Id
    public IActionResult DeliveryNoteUpdate(Guid id)
    {
        var note = _db.DeliveryNotes.Find(id);
        if (note == null) return NotFound();
        return View(note.ToViewDto());
    }

    // POST: /DeliveryNote/DeliveryNoteUpdate/5
    [HttpPost]
    public IActionResult DeliveryNoteUpdate(DeliveryNoteUdateDto model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var note = _db.DeliveryNotes.Find(model.Id);
                if (note == null) return NotFound();

                ;
                note.Update(model.DeliveryTime, model.Note, _currentUser.UserId);
                _db.DeliveryNotes.Update(note);
                _db.SaveChanges();
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi chỉnh sửa phiếu giao.");
            }
        }
        return RedirectToAction("DeliveryNotePage");
    }
}
