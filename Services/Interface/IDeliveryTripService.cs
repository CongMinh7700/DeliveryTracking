using DeliveryTrackingApp.Models;

public interface IDeliveryTripService
{
    Task<ServiceResult> CreateDeliveryTripAsync(DeliveryTrip.ViewDto model);
}

public class ServiceResult
{
    public bool Success { get; set; }
    public string Message { get; set; }
    public object Data { get; set; }

    public static ServiceResult Ok(string message = null, object data = null)
    {
        return new ServiceResult { Success = true, Message = message, Data = data };
    }

    public static ServiceResult Error(string message)
    {
        return new ServiceResult { Success = false, Message = message };
    }
}