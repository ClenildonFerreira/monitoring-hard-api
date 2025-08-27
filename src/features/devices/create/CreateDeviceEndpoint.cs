using Carter;
using Carter.ModelBinding;
using Microsoft.EntityFrameworkCore;
using MonitoringHardApi.Infrastructure.Database;
using MonitoringHardApi.Infrastructure.Iot;
using MonitoringHardApi.Shared.Domain;

namespace MonitoringHardApi.Features.Devices.Create;

public class CreateDeviceEndpoint : ICarterModule
{
    public void AddRoutes(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/devices", async (
            CreateDeviceRequest request,
            ApplicationDbContext db,
            IIotClient iotClient,
            HttpContext context) =>
        {
            var validator = new CreateDeviceValidator();
            var validationResult = await validator.ValidateAsync(request);
            
            if (!validationResult.IsValid)
                return Results.ValidationProblem(validationResult.ToDictionary());

            var device = new Device
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Location = request.Location,
                CreatedAt = DateTime.UtcNow
            };
            db.Devices.Add(device);
            await db.SaveChangesAsync();

            try
            {
                var callbackUrl = $"{context.Request.Scheme}://{context.Request.Host}/api/events";
                var integrationId = await iotClient.RegisterDeviceAsync(
                    device.Name,
                    device.Location,
                    callbackUrl);

                device.IntegrationId = integrationId;
                device.UpdatedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();
            }
            catch (Exception)
            {
                db.Devices.Remove(device);
                await db.SaveChangesAsync();

                return Results.Problem(
                    "Não foi possível registrar o dispositivo",
                    statusCode: StatusCodes.Status502BadGateway);
            }

            return Results.Created($"/api/devices/{device.Id}", device);
        });
    }
}