using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace DeliveryTrackingApp.Controllers;

using Constants;
using Enums;
using Models;
using Services.Interface;
using static Models.DeliveryTrip;

public class DeliveryTripController : Controller
{
    private readonly ILogger<DeliveryTripController> _logger;
    private readonly DeliveryDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public DeliveryTripController(ILogger<DeliveryTripController> logger, DeliveryDbContext db, ICurrentUserService currentUser)
    {
        _logger = logger;
        _db = db;
        _currentUser = currentUser;
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

            trip.Delete(_currentUser.UserId);//Replace to AdminId
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
            // Lấy thông tin phiếu giao hàng
            var deliveryNote = _db.DeliveryNotes
                .FirstOrDefault(p => p.Id == model.DeliveryNoteId && !p.IsDeleted);

            if (deliveryNote == null)
            {
                ModelState.AddModelError("", "Không tìm thấy phiếu giao hàng.");
                LoadDropdowns(model.UserId, model.Type, model.DeliveryNoteId);
                return View(model);
            }

            var now = DateTime.Now;
            var deliveryStatus = (int)TripStatus.OnTime;
            var deliveryTrip = _db.DeliveryTrips.OrderByDescending(p => p.CreatedOn).Where(p => !p.IsDeleted);
            var departureTrip = deliveryTrip.FirstOrDefault(p => p.TripType == (int)TripType.Departure && p.UserId == model.UserId);

            var existDeliveryTrip = deliveryTrip.FirstOrDefault(p => p.DeliveryNoteId == model.DeliveryNoteId && !p.IsDeleted);

            bool isDeparture = model.Type == (int)TripType.Departure;
            bool isReturn = model.Type == (int)TripType.Return;
            if (existDeliveryTrip != null)
            {
                bool isSameUserDuplicateDeparture =
                    existDeliveryTrip.UserId == model.UserId &&
                    existDeliveryTrip.TripType == (int)TripType.Departure &&
                    isDeparture;

                bool isOtherUserHandling = existDeliveryTrip.UserId != model.UserId;
                if (isSameUserDuplicateDeparture || isOtherUserHandling)
                {
                    LoadDropdowns(model.UserId, model.Type, model.DeliveryNoteId);
                    ModelState.AddModelError("", "Đơn hàng này đang được giao.");
                    return View(model);
                }
            }
            else if (existDeliveryTrip == null && isReturn)
            {
                LoadDropdowns(model.UserId, model.Type, model.DeliveryNoteId);
                ModelState.AddModelError("", "Đơn hàng chưa được giao.");
                return View(model);
            }

            deliveryStatus = departureTrip != null ? departureTrip.Status : CalculateDeliveryStatus(model.UserId, deliveryNote, now);

            // Tạo mới chuyến đi 
            var newDeliveryTrip = DeliveryTrip.Create(model.UserId, model.DeliveryNoteId, model.Type, deliveryStatus, _currentUser.UserId, now);

            _db.DeliveryTrips.Add(newDeliveryTrip);

            // Cập nhật trạng thái cho phiếu giao
            var noteStatus = model.Type == (int)TripType.Departure
                ? (int)NoteStatus.Delivering
                : (int)NoteStatus.Delivered;

            deliveryNote.UpdateStatus(noteStatus, _currentUser.UserId);
            _db.DeliveryNotes.Update(deliveryNote);

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

                trip.Update(model.Type, model.NoteId, _currentUser.UserId);
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
    /// <param name="selectedDriver"></param>
    /// <param name="selectedTripType"></param>
    /// <param name="slectedDeliveryNote"></param>
    private void LoadDropdowns(object selectedDriver = null, object selectedTripType = null, object slectedDeliveryNote = null)
    {
        // TripType
        var types = EnumHelper.GetIntListWithDescriptions<TripType>();
        ViewBag.TripTypes = new SelectList(types, "Key", "Value", selectedTripType);

        // Drivers
        var drivers = _db.Users.Include(u => u.Role)
            .Where(u => !u.IsDeleted && u.Role != null && u.Role.Name != RoleString.Admin)
            .Select(u => u.ToSearchCbDto()).ToList();
        ViewBag.Users = new SelectList(drivers, "UserId", "FullName", selectedDriver);

        var selectedUserId = selectedDriver?.ToString();
        if (selectedUserId == null)
        {
            selectedUserId = drivers.First().UserId;
        }

        var rawNotes = _db.DeliveryNotes.Where(p => !p.IsDeleted).Include(p => p.DeliveryTrips).ToList();

        var deliveryNotes = rawNotes
            .Where(p =>
                p.Status == (int)NoteStatus.Pending ||
                (
                    !string.IsNullOrEmpty(selectedUserId)
                    && p.DeliveryTrips.Any()
                    && p.DeliveryTrips.OrderByDescending(t => t.CreatedOn).First() is var latestTrip
                    && latestTrip.TripType == (int)TripType.Departure
                    && latestTrip.UserId == selectedUserId
                )
            ).Select(p => p.ToSearchCbDto()).ToList();

        ViewBag.DeliveryNotes = new SelectList(deliveryNotes, "DeliveryNoteId", "Code", selectedDriver);
    }

    /// <summary>
    /// CalculateDeliveryStatus
    /// </summary>
    /// <param name="userId"></param>
    /// <param name="deliveryNote"></param>
    /// <param name="now"></param>
    /// <returns></returns>
    private int CalculateDeliveryStatus(string userId, DeliveryNote deliveryNote, DateTime now)
    {
        var deliveryTime = now;// TG tài xế giao hàng
        var requiredTime = deliveryNote.DeliveryTime;// Thời gian giao hàng trên phiếu
        var deadline = requiredTime; //Thời gian cần giao thực tế
        var deliveryDate = requiredTime.Date;

        // Thời điểm hết tài
        var alert = _db.DriverAlerts.Where(a => a.CreatedOn <= deliveryTime).OrderByDescending(a => a.CreatedOn).FirstOrDefault();
        var readyTime = _db.DeliveryTrips.Where(t => t.UserId == userId && t.TripType == (int)TripType.Return && !t.IsDeleted)
            .OrderByDescending(t => t.CreatedOn)
            .Select(t => (DateTime?)t.CreatedOn)
            .FirstOrDefault();

        if (deliveryDate < now.Date && (!readyTime.HasValue || readyTime < deadline))
        {
            int offsetDays = (now.Date - deliveryDate).Days;
            deadline = deliveryDate.AddDays(offsetDays).Add(Setting.TimeToWork.ToTimeSpan());
        }
        else if (alert?.CreatedOn > requiredTime)
        {
            deadline = alert.CreatedOn;
        }
        else
        {
            deadline = readyTime.Value;
        }

        var isLate = deliveryTime >= deadline.AddMinutes(Setting.LateDelivery);
        return isLate ? (int)TripStatus.Late : (int)TripStatus.OnTime;
    }
}
