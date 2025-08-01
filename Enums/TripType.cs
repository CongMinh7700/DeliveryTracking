using System.ComponentModel;

namespace DeliveryTrackingApp.Enums;

public enum TripType
{
    /// <summary>
    /// Departure
    /// </summary>
    [Description("Đi")]
    Departure = 0,

    /// <summary>
    /// Return
    /// </summary>
    [Description("Về")]
    Return = 1
}
