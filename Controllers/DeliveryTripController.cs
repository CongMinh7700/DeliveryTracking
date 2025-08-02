using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DeliveryTrackingApp.Controllers;

using Enums;
using Models;
using static Models.DeliveryTrip;

public class DeliveryTripController : Controller
{
    private readonly ILogger<DeliveryTripController> _logger;
    private readonly DeliveryDbContext _db;

    public DeliveryTripController(ILogger<DeliveryTripController> logger, DeliveryDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet]
    public IActionResult DeliveryTripPage(string? keyword)
    {
        var query = _db.DeliveryTrips.Where(p => !p.IsDeleted).Include(p => p.DeliveryNote);

        var trips = query.Select(p => p.ToSearchDto()).ToList();

        return View(trips);
    }

    // POST: /DeliveryTrip/Delete/5
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Delete(Guid id)
    {
        var trip = _db.DeliveryTrips.FirstOrDefault(p => p.Id == id && p.IsDeleted == false);
        if (trip != null)
        {
            var createBy = User.FindFirst("UserId")?.Value;
            trip.Delete(createBy);//Replace to AdminId
            _db.SaveChanges();
            TempData["Message"] = "Xóa chuyến đi thành công";
        }
        else
        {
            TempData["Error"] = "Không tìm thấy chuyến đi này";
        }
        return RedirectToAction("DeliveryTripPage");
    }

    // GET: /DeliveryTrip/Create
    public IActionResult DeliveryTripCreate()
    {
        LoadDropdowns();
        return View();
    }

    // POST: /DeliveryTrip/DeliveryTripCreate
    [HttpPost]
    public IActionResult DeliveryTripCreate(DeliveryTrip.ViewDto model)
    {
        if (Request.Form["__RequestVerificationToken"].Any() && !ModelState.IsValid)
        {
            LoadDropdowns(model.UserId, model.Type, model.DeliveryNoteId);
            return View(model);
        }

        try
        {
            var createBy = User.FindFirst("UserId")?.Value;
            var newDeliveryTrip = DeliveryTrip.Create(model.UserId, model.DeliveryNoteId, model.Type, createBy);
            _db.DeliveryTrips.Add(newDeliveryTrip);

            //Update trạng thái phiếu giao
            var deliveryNote = _db.DeliveryNotes.Where(p => p.Id == model.DeliveryNoteId && !p.IsDeleted).FirstOrDefault();
            if (deliveryNote != null)
            {
                var status = model.Type == (int)TripType.Departure ? (int)DeliveryNoteStatus.Delivering : (int)DeliveryNoteStatus.Delivered;
                deliveryNote.UpdateStatus(status, "AD01");
                _db.DeliveryNotes.Update(deliveryNote);
            }

            _db.SaveChanges();

            return RedirectToAction("DeliveryTripPage");
        }
        catch
        {
            ModelState.AddModelError("", "Có lỗi xảy ra khi tạo chuyến đi.");
        }

        LoadDropdowns(model.UserId, model.Type, model.DeliveryNoteId);
        return View(model);
    }

    // GET: /DeliveryTrip/DeliveryTripUpdate/Id
    [Authorize(Roles = "Admin")]
    public IActionResult DeliveryTripUpdate(Guid id)
    {
        var trip = _db.DeliveryTrips.Include(p => p.DeliveryNote).FirstOrDefault(p => p.Id == id && !p.IsDeleted);
        if (trip == null) return NotFound();
        LoadDropdowns(trip.UserId, trip.TripType, trip.DeliveryNoteId);
        return View(trip.ToViewDto());
    }

    // POST: /DeliveryTrip/DeliveryTripUpdate/5
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult DeliveryTripUpdate(DeliveryTripUdateDto model)
    {
        LoadDropdowns(model.UserId, model.Type);

        if (ModelState.IsValid)
        {
            try
            {
                var trip = _db.DeliveryTrips.Include(p => p.DeliveryNote).FirstOrDefault(p => p.Id == model.Id && !p.IsDeleted);
                if (trip == null) return NotFound();

                var createBy = User.FindFirst("UserId")?.Value;
                trip.Update(model.Type, model.NoteId, createBy);
                _db.DeliveryTrips.Update(trip);
                _db.SaveChanges();
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi chỉnh sửa chuyến đi.");
            }
        }
        return RedirectToAction("DeliveryTripPage");
    }

    /// <summary>
    /// LoadDropdowns
    /// </summary>
    /// <param name="selectedUser"></param>
    /// <param name="selectedTripType"></param>
    /// <param name="slectedDeliveryNote"></param>
    private void LoadDropdowns(object selectedUser = null, object selectedTripType = null, object slectedDeliveryNote = null)
    {
        var types = EnumHelper.GetIntListWithDescriptions<TripType>();
        ViewBag.TripTypes = new SelectList(types, "Key", "Value", selectedTripType);

        var users = _db.Users.Where(p => !p.IsDeleted).Select(p => p.ToSearchCbDto()).ToList();
        ViewBag.Users = new SelectList(users, "UserId", "FullName", selectedUser);

        var selectedUserId = selectedUser?.ToString();
        if (selectedUserId == null)
        {
            selectedUserId = users.First().UserId;
        }

        var rawNotes = _db.DeliveryNotes
            .Where(p => !p.IsDeleted && p.DeliveryTime <= DateTime.Now)
            .Include(p => p.DeliveryTrips)
            .ToList();

        var deliveryNotes = rawNotes
            .Where(p =>
                p.Status == (int)DeliveryNoteStatus.Pending ||
                (
                    !string.IsNullOrEmpty(selectedUserId)
                    && p.DeliveryTrips.Any()
                    && p.DeliveryTrips.OrderByDescending(t => t.CreatedOn).First() is var latestTrip
                    && latestTrip.TripType == (int)TripType.Departure
                    && latestTrip.UserId == selectedUserId
                )
            ).Select(p => p.ToSearchCbDto()).ToList();

        ViewBag.DeliveryNotes = new SelectList(deliveryNotes, "DeliveryNoteId", "Code", selectedUser);
    }
}
