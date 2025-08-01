using Microsoft.AspNetCore.SignalR;

namespace DeliveryTrackingApp.Hubs;

public class DriverStatusHub : Hub
{
    public async Task SendStatusUpdate(string userId, string status)
    {
        await Clients.All.SendAsync("ReceiveDriverStatus", userId, status);
    }
}
