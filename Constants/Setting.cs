namespace DeliveryTrackingApp.Constants;

public class Setting
{
    /// <summary>
    /// LateDelivery
    /// </summary>
    public const float LateDelivery = 15;

    /// <summary>
    /// TimeToWork
    /// </summary>
    public static readonly TimeOnly TimeToWork = new TimeOnly(8, 0);

    /// <summary>
    /// TimeOff
    /// </summary>
    public static readonly TimeOnly TimeOff = new TimeOnly(17, 45);
}