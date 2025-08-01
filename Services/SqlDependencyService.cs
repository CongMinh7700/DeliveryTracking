using Microsoft.AspNetCore.SignalR;
using Microsoft.Data.SqlClient;

namespace DeliveryTrackingApp.Services;

using Hubs;
using static Models.User;

public class SqlDependencyService
{
    private readonly IHubContext<DriverStatusHub> _hubContext;
    private readonly string _connectionString = "Server=.\\SQLEXPRESS;Database=DeliveryTracking;Trusted_Connection=True;TrustServerCertificate=True;";

    private SqlConnection _connection;
    private SqlCommand _command;
    private SqlDependency _dependency;

    public SqlDependencyService(IHubContext<DriverStatusHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public void Start()
    {
        SqlDependency.Start(_connectionString);

        _connection = new SqlConnection(_connectionString);
        _connection.Open();

        _command = new SqlCommand("SELECT UserId, TripType FROM dbo.DeliveryTrips", _connection);
        _dependency = new SqlDependency(_command);
        _dependency.OnChange += async (s, e) => await OnTripsChangedAsync();

        _command.ExecuteReader(); // bắt buộc
    }

    private async Task OnTripsChangedAsync()
    {
        _dependency.OnChange -= async (s, e) => await OnTripsChangedAsync(); // tránh gọi nhiều lần

        // Gửi trạng thái mới
        var driverStatusList = GetDriverStatuses();
        await _hubContext.Clients.All.SendAsync("ReceiveDriverStatusList", driverStatusList);

        if (driverStatusList.All(d => d.Status == "Bận"))
        {
            var message = $"⚠️ Hết tài xế lúc {DateTime.Now:HH:mm:ss dd/MM/yyyy}";
            await _hubContext.Clients.All.SendAsync("ReceiveDriverAlert", message);

            // Lưu vào DB
            using (var connection = new SqlConnection(_connectionString))
            {
                var insertQuery = "INSERT INTO DriverAlerts (Message) VALUES (@Message)";
                using (var cmd = new SqlCommand(insertQuery, connection))
                {
                    cmd.Parameters.AddWithValue("@Message", message);
                    connection.Open();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        // Đăng ký lại dependency mới (không dùng lại cái cũ)
        Start();
    }

    private List<SearchDto> GetDriverStatuses()
    {
        var result = new List<SearchDto>();

        using var connection = new SqlConnection(_connectionString);
        connection.Open();

        var query = @"
        SELECT 
            u.Id,
            u.FullName,
            u.CreatedOn,
            CASE
                WHEN latest.TripType = 0 THEN N'Bận'
                ELSE N'Rảnh' -- TripType = 1 hoặc NULL (không có chuyến) đều là Rảnh
            END AS Status
        FROM Users u
        OUTER APPLY (
            SELECT TOP 1 d.TripType
            FROM DeliveryTrips d
            WHERE d.UserId = u.Id AND d.IsDeleted = 0
            ORDER BY d.CreatedOn DESC
        ) AS latest 
        WHERE u.IsDeleted = 0 AND u.Id != 'AD01'";

        using var command = new SqlCommand(query, connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            result.Add(new SearchDto
            {
                Id = reader.GetString(0),
                FullName = reader.GetString(1),
                CreatedOn = reader.IsDBNull(2) ? (DateTime?)null : reader.GetDateTime(2),
                Status = reader.GetString(3)
            });
        }

        return result;
    }
}
