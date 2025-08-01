using Microsoft.AspNetCore.Mvc;

namespace DeliveryTrackingApp.Controllers;

using Models;

[ApiController]
[Route("api/DriverAlert")]
public class DriverAlertController : Controller
{
    private readonly ILogger<DriverAlertController> _logger;
    private readonly DeliveryDbContext _db;

    public DriverAlertController(ILogger<DriverAlertController> logger, DeliveryDbContext db)
    {
        _logger = logger;
        _db = db;
    }

    [HttpGet("GetAlerts")]
    public IActionResult GetAlerts()
    {
        var alerts = _db.DriverAlerts.OrderByDescending(a => a.CreatedOn).Take(10).ToList();

        return Ok(alerts);
    }
}
