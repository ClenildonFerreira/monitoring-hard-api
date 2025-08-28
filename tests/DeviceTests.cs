namespace MonitoringHardApi.Tests;

public class DeviceTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DeviceTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateDevice_Should_Return_Created_And_Register_With_IotProvider()
    {
        var mockIot = new Mock<IIotClient>();
        mockIot.Setup(x => x.RegisterDeviceAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("external-id-123");

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.Single(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                services.Remove(descriptor);
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("testdb_create"));

                services.AddSingleton<IIotClient>(mockIot.Object);
            });
        }).CreateClient();

        var res = await client.PostAsJsonAsync("/api/devices", new { name = "Dev1", location = "Lab" });
        res.StatusCode.Should().Be(HttpStatusCode.Created);

        var body = await res.Content.ReadFromJsonAsync<Device>();
        body.Should().NotBeNull();
        body!.IntegrationId.Should().Be("external-id-123");
    }

    [Fact]
    public async Task CreateEvent_For_Nonexistent_Device_Should_Return_404()
    {
        var client = _factory.CreateClient();

        var res = await client.PostAsJsonAsync("/api/events", new { integrationId = "nonexistent", temperature = 10, humidity = 10, isAlarm = false, timestamp = System.DateTime.UtcNow });
        res.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task DeleteDevice_Should_Call_Unregister_When_HasIntegrationId()
    {
        var mockIot = new Mock<IIotClient>();
        mockIot.Setup(x => x.UnregisterDeviceAsync("ext-id")).Returns(Task.CompletedTask).Verifiable();

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.Single(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                services.Remove(descriptor);
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("testdb_delete"));

                services.AddSingleton<IIotClient>(mockIot.Object);

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Devices.Add(new Device { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "D1", Location = "L", IntegrationId = "ext-id", CreatedAt = DateTime.UtcNow });
                db.SaveChanges();
            });
        }).CreateClient();

        var res = await client.DeleteAsync("/api/devices/00000000-0000-0000-0000-000000000001");
        res.StatusCode.Should().Be(HttpStatusCode.NoContent);
        mockIot.Verify();
    }

    [Fact]
    public async Task CreateDevice_When_Iot_Register_Fails_Should_Rollback_And_Return_502()
    {
        var mockIot = new Mock<IIotClient>();
        mockIot.Setup(x => x.RegisterDeviceAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new System.Exception("fail"));

        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.Single(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                services.Remove(descriptor);
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("testdb_create_fail"));

                services.AddSingleton<IIotClient>(mockIot.Object);
            });
        }).CreateClient();

        var res = await client.PostAsJsonAsync("/api/devices", new { name = "Dev1", location = "Lab" });
        res.StatusCode.Should().Be((System.Net.HttpStatusCode)502);

        var sp = _factory.Services;
        using var scope = sp.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var count = await db.Devices.CountAsync();
        count.Should().Be(0);
    }

    [Fact]
    public async Task PatchDevice_PartialUpdate_Should_Update_Name_Only()
    {
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.Single(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                services.Remove(descriptor);
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("testdb_patch_partial"));

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Devices.Add(new Device { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "OldName", Location = "OldLoc", CreatedAt = DateTime.UtcNow });
                db.SaveChanges();
            });
        }).CreateClient();

        var patchReq = new HttpRequestMessage(new HttpMethod("PATCH"), "/api/devices/00000000-0000-0000-0000-000000000002")
        {
            Content = JsonContent.Create(new { name = "NewName" })
        };

        var res = await client.SendAsync(patchReq);
        res.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await res.Content.ReadFromJsonAsync<dynamic>();
        string name = body.name ?? body.Name ?? "";
        name.Should().Be("NewName");
    }

    [Fact]
    public async Task PatchDevice_InvalidNameTooLong_Should_Return_BadRequest()
    {
        var client = _factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                var descriptor = services.Single(s => s.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                services.Remove(descriptor);
                services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("testdb_patch_invalid"));

                var sp = services.BuildServiceProvider();
                using var scope = sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                db.Devices.Add(new Device { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "N", Location = "L", CreatedAt = DateTime.UtcNow });
                db.SaveChanges();
            });
        }).CreateClient();

        var longName = new string('A', 101);
        var patchReq = new HttpRequestMessage(new HttpMethod("PATCH"), "/api/devices/00000000-0000-0000-0000-000000000003")
        {
            Content = JsonContent.Create(new { name = longName })
        };

        var res = await client.SendAsync(patchReq);
        res.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}