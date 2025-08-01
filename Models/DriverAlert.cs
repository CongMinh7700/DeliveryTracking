namespace DeliveryTrackingApp.Models;

public partial class DriverAlert
{
    public Guid Id { get; set; }

    public string? Message { get; set; }

    public DateTime CreatedOn { get; set; }
}
