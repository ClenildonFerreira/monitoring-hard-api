namespace MonitoringHardApi.Tests.Unit;

public class UnitDeviceManagerTests
{
    [Fact]
    public async Task CreateAsync_Success_ReturnsDeviceWithIntegrationId()
    {
        var repoMock = new Mock<IDeviceRepository>();
        repoMock.Setup(r => r.AddAsync(It.IsAny<Device>())).Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var iotMock = new Mock<IIotClient>();
        iotMock.Setup(i => i.RegisterDeviceAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("int-1");

        var mgr = new DeviceManager(repoMock.Object, iotMock.Object);

        var device = await mgr.CreateAsync("Name", "Loc", "http://callback");

        device.IntegrationId.Should().Be("int-1");
        repoMock.Verify(r => r.AddAsync(It.IsAny<Device>()), Times.Once);
        repoMock.Verify(r => r.SaveChangesAsync(), Times.AtLeastOnce);
    }

    [Fact]
    public async Task CreateAsync_WhenIoTRegisterFails_ShouldBubbleExceptionAndNotSetIntegration()
    {
        var repoMock = new Mock<IDeviceRepository>();
        repoMock.Setup(r => r.AddAsync(It.IsAny<Device>())).Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var iotMock = new Mock<IIotClient>();
        iotMock.Setup(i => i.RegisterDeviceAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new System.Exception("fail"));

        var mgr = new DeviceManager(repoMock.Object, iotMock.Object);

        await Assert.ThrowsAsync<System.Exception>(() => mgr.CreateAsync("Name", "Loc", "http://callback"));
    }

    [Fact]
    public async Task DeleteAsync_WithIntegrationId_CallsUnregisterAndRemoves()
    {
        var repoMock = new Mock<IDeviceRepository>();
        repoMock.Setup(r => r.RemoveAsync(It.IsAny<Device>())).Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var iotMock = new Mock<IIotClient>();
        iotMock.Setup(i => i.UnregisterDeviceAsync("ext-1")).Returns(Task.CompletedTask).Verifiable();

        var mgr = new DeviceManager(repoMock.Object, iotMock.Object);

        var device = new Device { Id = Guid.NewGuid(), Name = "D", Location = "L", IntegrationId = "ext-1", CreatedAt = DateTime.UtcNow };

        await mgr.DeleteAsync(device);

        iotMock.Verify();
        repoMock.Verify(r => r.RemoveAsync(device), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WithoutIntegrationId_JustRemoves()
    {
        var repoMock = new Mock<IDeviceRepository>();
        repoMock.Setup(r => r.RemoveAsync(It.IsAny<Device>())).Returns(Task.CompletedTask);
        repoMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

        var iotMock = new Mock<IIotClient>();

        var mgr = new DeviceManager(repoMock.Object, iotMock.Object);

        var device = new Device { Id = Guid.NewGuid(), Name = "D", Location = "L", IntegrationId = null, CreatedAt = DateTime.UtcNow };

        await mgr.DeleteAsync(device);

        repoMock.Verify(r => r.RemoveAsync(device), Times.Once);
        iotMock.Verify(i => i.UnregisterDeviceAsync(It.IsAny<string>()), Times.Never);
    }
}