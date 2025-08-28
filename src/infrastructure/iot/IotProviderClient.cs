using System.Text.Json;

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
        private readonly ILogger<IotProviderClient> _logger;

        public IotProviderClient(HttpClient httpClient, ILogger<IotProviderClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }
        public async Task<string> RegisterDeviceAsync(string name, string location, string callbackUrl)
        {
            var request = new
            {
                deviceName = name,
                location = location,
                callbackUrl = callbackUrl
            };
            
            try
            {
                var reqJson = JsonSerializer.Serialize(request);
                _logger.LogDebug("IotProviderClient Register request: {Request}", reqJson);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to serialize register request for logging");
            }

            var response = await _httpClient.PostAsJsonAsync("register", request);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            try
            {
                _logger.LogDebug("IotProviderClient Register response: {Response}", content);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to log register response");
            }

            if (string.IsNullOrWhiteSpace(content))
                throw new InvalidOperationException("Resposta vazia do provedor IoT ao registrar dispositivo");

            try
            {
                var integrationId = JsonSerializer.Deserialize<string>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                if (!string.IsNullOrWhiteSpace(integrationId))
                    return integrationId;
            }
            catch (JsonException ex)
            {
                _logger.LogDebug(ex, "Resposta do provedor IoT não é uma string JSON simples");
            }

            try
            {
                using var doc = JsonDocument.Parse(content);
                var root = doc.RootElement;

                if (root.ValueKind == JsonValueKind.String)
                {
                    var id = root.GetString();
                    if (!string.IsNullOrWhiteSpace(id))
                        return id;
                }

                if (root.ValueKind == JsonValueKind.Object)
                {
                    if (root.TryGetProperty("integrationId", out var prop) && prop.ValueKind == JsonValueKind.String)
                        return prop.GetString()!;

                    if (root.TryGetProperty("id", out prop) && prop.ValueKind == JsonValueKind.String)
                        return prop.GetString()!;

                    foreach (var property in root.EnumerateObject())
                    {
                        if (property.Value.ValueKind == JsonValueKind.String)
                            return property.Value.GetString()!;
                    }
                }
            }
            catch (JsonException ex)
            {
                _logger.LogDebug(ex, "Falha ao parsear resposta JSON");
            }

            var trimmed = content.Trim();
            if (trimmed.Length >= 2 && trimmed.StartsWith("\"") && trimmed.EndsWith("\""))
                trimmed = trimmed.Substring(1, trimmed.Length - 2);

            if (!string.IsNullOrWhiteSpace(trimmed))
                return trimmed;

            throw new InvalidOperationException("Integration ID não pôde ser determinado a partir da resposta do provedor IoT");
        }

        public async Task UnregisterDeviceAsync(string integrationId)
        {
            var url = $"unregister/{integrationId}";
            _logger.LogDebug("IotProviderClient Unregister request: {Url}", url);

            var response = await _httpClient.DeleteAsync(url);
            var content = await response.Content.ReadAsStringAsync();

            try
            {
                _logger.LogDebug("IotProviderClient Unregister response: {Response}", content);
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex, "Failed to log unregister response");
            }

            response.EnsureSuccessStatusCode();
        }
    }
}