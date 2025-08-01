namespace DeliveryTrackingApp.Requests;

public class RegisterR
{
    /// <summary>
    /// Username
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// FullName
    /// </summary>
    public string? FullName { get; set; }

    /// <summary>
    /// RePassword
    /// </summary>
    public string? RePassword { get; set; }
}