using Carter;
using Carter.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MonitoringHardApi.Infrastructure.Database;

namespace MonitoringHardApi.Features.Devices.Update;

public class UpdateDeviceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
    app.MapPatch("/api/devices/{id}", async (
            Guid id,
            UpdateDeviceRequest request,
            ApplicationDbContext db) =>
        {
            var validator = new UpdateDeviceValidator();
            var validationResult = await validator.ValidateAsync(request);
            
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            var device = await db.Devices.FindAsync(id);

            if (device == null)
                return Results.NotFound("Dispositivo não encontrado");

            if (request.Name != null)
                device.Name = request.Name;

            if (request.Location != null)
                device.Location = request.Location;
            device.UpdatedAt = DateTime.UtcNow;

            await db.SaveChangesAsync();

            return Results.Ok(new
            {
                device.Id,
                device.Name,
                device.Location,
                device.IntegrationId,
                device.CreatedAt,
                device.UpdatedAt
            });
        });
    }
}