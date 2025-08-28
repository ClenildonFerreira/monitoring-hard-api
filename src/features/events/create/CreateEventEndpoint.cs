using Carter;
using Microsoft.EntityFrameworkCore;
using MonitoringHardApi.Infrastructure.Database;
using MonitoringHardApi.Shared.Domain;
using Microsoft.AspNetCore.SignalR;

namespace MonitoringHardApi.Features.Events.Create;

public class CreateEventEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/events", async (
            CreateEventRequest request,
            ApplicationDbContext db,
            IHubContext<MonitoringHardApi.Infrastructure.Signaling.EventHub> hub) =>
        {
            var validator = new CreateEventValidator();
            var validationResult = await validator.ValidateAsync(request);
            
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            var device = await db.Devices
                .FirstOrDefaultAsync(d => d.IntegrationId == request.IntegrationId);

            if (device == null)
                return Results.NotFound("Dispositivo não encontrado");

            var @event = new Event
            {
                Id = Guid.NewGuid(),
                DeviceId = device.Id,
                Temperature = request.Temperature,
                Humidity = request.Humidity,
                IsAlarm = request.IsAlarm,
                Timestamp = request.Timestamp
            };

            db.Events.Add(@event);
            await db.SaveChangesAsync();

            // Publish to SignalR hub for real-time clients
            try
            {
                await hub.Clients.All.SendAsync("EventReceived", new
                {
                    @event.Id,
                    @event.Temperature,
                    @event.Humidity,
                    @event.IsAlarm,
                    @event.Timestamp,
                    Device = new { device.Id, device.Name, device.Location }
                });
            }
            catch { /* swallow hub errors to not break the API */ }

            return Results.Ok(new
            {
                @event.Id,
                @event.Temperature,
                @event.Humidity,
                @event.IsAlarm,
                @event.Timestamp,
                Device = new
                {
                    device.Id,
                    device.Name,
                    device.Location
                }
            });
        });
    }
}