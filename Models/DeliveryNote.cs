using System;
using System.Collections.Generic;

namespace DeliveryTrackingApp.Models;

public partial class DeliveryNote
{
    public Guid Id { get; set; }

    public string Code { get; set; } = null!;

    public DateTime DeliveryTime { get; set; }

    public int Status { get; set; }

    public string? Note { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public string? ModifiedBy { get; set; }

    public bool IsDeleted { get; set; }

    public virtual ICollection<DeliveryTrip> DeliveryTrips { get; set; } = new List<DeliveryTrip>();
}
