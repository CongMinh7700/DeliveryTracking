using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

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
        var query = _db.DeliveryTrips.Where(p => !p.IsDeleted);

        var trips = query.Select(p => p.ToSearchDto()).ToList();

        return View(trips);
    }

    // POST: /DeliveryTrip/Delete/5
    [HttpPost]
    public IActionResult Delete(Guid id)
    {
        var trip = _db.DeliveryTrips.FirstOrDefault(p => p.Id == id && p.IsDeleted == false);
        if (trip != null)
        {
            trip.Delete("AD01");//Replace to AdminId
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
    public IActionResult DeliveryTripCreate(Models.DeliveryTrip.ViewDto model)
    {
        if (!ModelState.IsValid)
        {
            LoadDropdowns(model.UserId, model.Type);
            return View();
        }

        if (ModelState.IsValid)
        {
            try
            {
                // Replace Admin =  tripID of Admin
                var newDeliveryTrip = Models.DeliveryTrip.Create(model.UserId, model.DeliveryNoteId, Enums.TripType.Departure, "Admin");
                _db.DeliveryTrips.Add(newDeliveryTrip);
                _db.SaveChanges();
                return RedirectToAction("DeliveryTripPage");
            }
            catch
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo chuyến đi.");
            }
        }
        return View(model);
    }

    // GET: /DeliveryTrip/DeliveryTripUpdate/Id
    public IActionResult DeliveryTripUpdate(Guid id)
    {
        var trip = _db.DeliveryTrips.Find(id);
        if (trip == null) return NotFound();
        LoadDropdowns(trip.UserId, trip.TripType);

        return View(trip.ToViewDto());
    }

    // POST: /DeliveryTrip/DeliveryTripUpdate/5
    [HttpPost]
    public IActionResult DeliveryTripUpdate(DeliveryTripUdateDto model)
    {
        LoadDropdowns(model.UserId, model.Type);

        if (ModelState.IsValid)
        {
            try
            {
                var trip = _db.DeliveryTrips.Find(model.Id);
                if (trip == null) return NotFound();

                trip.Update(model.Type, model.NoteId, "AD01");
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

    private void LoadDropdowns(object selectedUser = null, object selectedTripType = null)
    {
        var types = EnumHelper.GetIntListWithDescriptions<TripType>();
        ViewBag.TripTypes = new SelectList(types, "Key", "Value", selectedTripType);

        var pumps = _db.Users.Where(p => !p.IsDeleted)
                        .Select(p => p.ToSearchCbDto()).ToList();
        ViewBag.Users = new SelectList(pumps, "UserId", "FullName", selectedUser);
    }
}
