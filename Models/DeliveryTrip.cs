namespace DeliveryTrackingApp.Models;

using Enums;

public partial class DeliveryTrip
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// UserId
    /// </summary>
    public string UserId { get; set; } = default!;

    /// <summary>
    /// TripType
    /// </summary>
    public TripType TripType { get; set; } // 0: Đi, 1: Về

    /// <summary>
    /// CreatedOn
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// CreatedBy
    /// </summary>
    public string? CreatedBy { get; set; }
}
