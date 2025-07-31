using Microsoft.EntityFrameworkCore;

namespace DeliveryTrackingApp.Models;
public partial class DeliveryContext : DbContext
{
    public DeliveryContext(DbContextOptions<DeliveryContext> options) : base(options) { }

    /// <summary>
    /// Users
    /// </summary>
    public DbSet<User> Users { get; set; }

    /// <summary>
    /// Roles
    /// </summary>
    public DbSet<Role> Roles { get; set; }

    /// <summary>
    /// DeliveryTrips
    /// </summary>
    public DbSet<DeliveryTrip> DeliveryTrips { get; set; }
}