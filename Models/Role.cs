namespace DeliveryTrackingApp.Models;

/// <summary>
/// Role
/// </summary>
public partial class Role
{
    /// <summary>
    /// Id
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// CreatedOn
    /// </summary>
    public DateTime CreatedOn { get; set; } = DateTime.Now;

    /// <summary>
    /// ModifiedOn
    /// </summary>
    public DateTime ModifiedOn { get; set; }
}
