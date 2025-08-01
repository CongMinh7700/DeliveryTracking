namespace DeliveryTrackingApp.Models;

public partial class User
{
    public string Id { get; set; } = null!;

    public Guid RoleId { get; set; }

    public string? Username { get; set; }

    public string? FullName { get; set; }

    public string? PhoneNumber { get; set; }

    public string? PasswordHash { get; set; }

    public DateOnly? BirthDate { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<DeliveryTrip> DeliveryTrips { get; set; } = new List<DeliveryTrip>();

    public virtual Role Role { get; set; } = null!;
}
