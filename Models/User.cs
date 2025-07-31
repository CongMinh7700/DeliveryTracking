namespace DeliveryTrackingApp.Models;

/// <summary>
/// User
/// </summary>
public partial class User
{
    /// <summary>
    /// Id
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    /// RoleId
    /// </summary>
    public Guid RoleId { get; set; }

    /// <summary>
    /// UserName
    /// </summary>
    public string? UserName { get; set; }

    /// <summary>
    /// PasswordHash
    /// </summary>
    public string? PasswordHash { get; set; }

    /// <summary>
    /// BirthDate
    /// </summary>
    public DateTime BirthDate { get; set; }

    /// <summary>
    /// CreateOn
    /// </summary>
    public DateTime CreateOn { get; set; }

    /// <summary>
    /// CreatedBy
    /// </summary>
    public string? CreatedBy { get; set; }

    /// <summary>
    /// ModifiedOn
    /// </summary>
    public DateTime ModifiedOn { get; set; }

    /// <summary>
    /// ModifiedBy
    /// </summary>
    public string? ModifiedBy { get; set; }
}