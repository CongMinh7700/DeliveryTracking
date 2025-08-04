using System.ComponentModel;

namespace DeliveryTrackingApp.Enums;

public enum TripStatus
{
    /// <summary>
    /// On time
    /// </summary>
    [Description("Đúng giờ")]
    OnTime = 0,

    /// <summary>
    /// Late delivery
    /// </summary>
    [Description("Trễ giờ")]
    Late = 1,
}

