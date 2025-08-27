using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonitoringHardApi.Infrastructure.Iot
{
    public interface IIotSimulatorClient
    {
        Task<string> RegisterDeviceAsync(string name, string location, string callbackUrl);
        Task UnregisterDeviceAsync(string integrationId);
    }

    public class IotSimulatorClient : IIotSimulatorClient
    {
        private readonly HttpClient _httpClient;

        public IotSimulatorClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://localhost:5000/");
        }

        public async Task<string> RegisterDeviceAsync(string name, string location, string callbackUrl)
        {
            var request = new
            {
                deviceName = name,
                location = location,
                callbackUrl = callbackUrl
            };

            var response = await _httpClient.PostAsJsonAsync("register", request);
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<string>(content);
        }

        public async Task UnregisterDeviceAsync(string integrationId)
        {
            var response = await _httpClient.DeleteAsync($"unregister/{integrationId}");
            response.EnsureSuccessStatusCode();
        }
    }
}
