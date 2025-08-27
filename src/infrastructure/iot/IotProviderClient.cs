using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace MonitoringHardApi.Infrastructure.Iot
{
    public interface IIotClient
    {
        Task<string> RegisterDeviceAsync(string name, string location, string callbackUrl);
        Task UnregisterDeviceAsync(string integrationId);
    }

    public class IotProviderClient : IIotClient
    {
        private readonly HttpClient _httpClient;

        public IotProviderClient(HttpClient httpClient)
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
            var integrationId = JsonSerializer.Deserialize<string>(content);
            return integrationId ?? throw new InvalidOperationException("Integration ID não pode ser nulo");
        }

        public async Task UnregisterDeviceAsync(string integrationId)
        {
            var response = await _httpClient.DeleteAsync($"unregister/{integrationId}");
            response.EnsureSuccessStatusCode();
        }
    }
}