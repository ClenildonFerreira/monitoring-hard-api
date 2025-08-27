using Carter;
using Microsoft.EntityFrameworkCore;
using MonitoringHardApi.Infrastructure.Database;

namespace MonitoringHardApi.Features.Devices.Get;

public class GetDeviceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapGet("/api/devices/{id}", async (
            Guid id,
            ApplicationDbContext db) =>
        {
            var device = await db.Devices
                .Include(d => d.Events.OrderByDescending(e => e.Timestamp).Take(10))
                .FirstOrDefaultAsync(d => d.Id == id);

            if (device == null)
                return Results.NotFound("Dispositivo não encontrado");

            return Results.Ok(new
            {
                device.Id,
                device.Name,
                device.Location,
                device.IntegrationId,
                device.CreatedAt,
                device.UpdatedAt,
                RecentEvents = device.Events.Select(e => new
                {
                    e.Id,
                    e.Temperature,
                    e.Humidity,
                    e.IsAlarm,
                    e.Timestamp
                })
            });
        });
    }
}