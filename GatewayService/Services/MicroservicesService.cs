using GatewayService.Models;
using System.Text.Json;

namespace GatewayService.Services
{
    public interface IMicroservicesService
    {
        Task<DolarResponse?> GetDolarAsync(string authToken);
        Task<DolarResponse?> GetDolarPublicAsync();
        Task<TemperaturaResponse?> GetTemperaturaAsync(string ciudad, string authToken);
        Task<TemperaturaResponse?> GetTemperaturaPublicAsync(string ciudad);
    }

    public class MicroservicesService : IMicroservicesService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MicroservicesService> _logger;

        public MicroservicesService(HttpClient httpClient, IConfiguration configuration, ILogger<MicroservicesService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<DolarResponse?> GetDolarAsync(string authToken)
        {
            try
            {
                var dolarServiceUrl = _configuration["MicroservicesUrls:DolarService"] ?? "http://localhost:5104";
                var requestUri = $"{dolarServiceUrl}/api/Dolar";

                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                request.Headers.Add("Authorization", authToken);

                var response = await _httpClient.SendAsync(request);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<DolarResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    _logger.LogError("Error al llamar al servicio de dólar. StatusCode: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar al servicio de dólar");
                return null;
            }
        }

        public async Task<DolarResponse?> GetDolarPublicAsync()
        {
            try
            {
                var dolarServiceUrl = _configuration["MicroservicesUrls:DolarService"] ?? "http://localhost:5104";
                var requestUri = $"{dolarServiceUrl}/api/Dolar/public";

                var response = await _httpClient.GetAsync(requestUri);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<DolarResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    _logger.LogError("Error al llamar al servicio público de dólar. StatusCode: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar al servicio público de dólar");
                return null;
            }
        }

        public async Task<TemperaturaResponse?> GetTemperaturaAsync(string ciudad, string authToken)
        {
            try
            {
                var temperaturaServiceUrl = _configuration["MicroservicesUrls:TemperaturaService"] ?? "http://localhost:5050";
                var requestUri = $"{temperaturaServiceUrl}/api/Temperatura/{ciudad}";

                var request = new HttpRequestMessage(HttpMethod.Get, requestUri);
                request.Headers.Add("Authorization", authToken);

                var response = await _httpClient.SendAsync(request);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<TemperaturaResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    _logger.LogError("Error al llamar al servicio de temperatura. StatusCode: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar al servicio de temperatura para la ciudad: {Ciudad}", ciudad);
                return null;
            }
        }

        public async Task<TemperaturaResponse?> GetTemperaturaPublicAsync(string ciudad)
        {
            try
            {
                var temperaturaServiceUrl = _configuration["MicroservicesUrls:TemperaturaService"] ?? "http://localhost:5050";
                var requestUri = $"{temperaturaServiceUrl}/api/Temperatura/public/{ciudad}";

                var response = await _httpClient.GetAsync(requestUri);
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<TemperaturaResponse>(content, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
                else
                {
                    _logger.LogError("Error al llamar al servicio público de temperatura. StatusCode: {StatusCode}", response.StatusCode);
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al llamar al servicio público de temperatura para la ciudad: {Ciudad}", ciudad);
                return null;
            }
        }
    }
}
