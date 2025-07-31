using Microsoft.AspNetCore.Mvc;

namespace DeliveryTrackingApp.Controllers;

public class DeliveryTripController : Controller
{
    private readonly ILogger<DeliveryTripController> _logger;

    public DeliveryTripController(ILogger<DeliveryTripController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
}
