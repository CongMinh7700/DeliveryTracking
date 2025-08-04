using System;
using System.Collections.Generic;

namespace DeliveryTrackingApp.Models;

public partial class DeliveryTrip
{
    public Guid Id { get; set; }

    public string UserId { get; set; } = null!;

    public Guid DeliveryNoteId { get; set; }

    public int TripType { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public int Status { get; set; }

    public virtual DeliveryNote DeliveryNote { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
