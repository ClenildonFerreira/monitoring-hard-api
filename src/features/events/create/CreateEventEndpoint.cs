using Carter;
using Carter.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MonitoringHardApi.Infrastructure.Database;
using MonitoringHardApi.Shared.Domain;

namespace MonitoringHardApi.Features.Events.Create;

public class CreateEventEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/events", async (
            CreateEventRequest request,
            ApplicationDbContext db) =>
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