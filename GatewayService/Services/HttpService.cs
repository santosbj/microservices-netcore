using GatewayService.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace GatewayService.Services
{
    public interface IHttpService
    {
        Task<HttpResponseMessage> ForwardRequestWithJwtAsync(string url, string token);
    }

    public class HttpService : IHttpService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HttpService> _logger;

        public HttpService(HttpClient httpClient, ILogger<HttpService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<HttpResponseMessage> ForwardRequestWithJwtAsync(string url, string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                _logger.LogInformation($"Enviando solicitud a: {url}");
                var response = await _httpClient.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning($"Error en la solicitud a {url}. Código: {response.StatusCode}");
                }

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al enviar la solicitud a {url}");
                throw;
            }
        }
    }
}