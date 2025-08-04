using System.ComponentModel;

namespace DeliveryTrackingApp.Enums;

public enum NoteStatus
{
    /// <summary>
    /// Departure
    /// </summary>
    [Description("Đang chờ")]
    Pending = 0,

    /// <summary>
    /// Return
    /// </summary>
    [Description("Đang giao")]
    Delivering = 1,

    /// <summary>
    /// Delivered
    /// </summary>
    [Description("Đã giao")]
    Delivered = 2,
}
