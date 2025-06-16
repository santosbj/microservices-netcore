using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Controllers
{
    /// <summary>
    /// Controlador de información general del Gateway
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InfoController : ControllerBase
    {
        private readonly ILogger<InfoController> _logger;
        private readonly IConfiguration _configuration;

        public InfoController(ILogger<InfoController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Obtiene información general del Gateway y servicios disponibles
        /// </summary>
        /// <returns>Información del Gateway y microservicios</returns>
        /// <response code="200">Información del Gateway</response>
        [HttpGet]
        [ProducesResponseType(200)]
        public IActionResult GetInfo()
        {
            var info = new
            {
                Gateway = new
                {
                    Name = "Microservicios Gateway",
                    Version = "1.0.0",
                    Environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                    Timestamp = DateTime.UtcNow
                },
                HealthChecks = new
                {
                    Endpoint = "/health",
                    UI = "/healthchecks-ui",
                    Description = "Interfaz visual para monitoreo en tiempo real de todos los microservicios"
                },
                Servicios = new
                {
                    DolarService = new
                    {
                        Name = "Servicio de Dólar",
                        Endpoint = "/api/Dolar",
                        Description = "Consulta el precio actual del dólar",
                        HealthCheck = "http://localhost:5104/health",
                        HealthUI = "http://localhost:5104/healthchecks-ui",
                        RequiresAuth = true
                    },
                    TemperaturaService = new
                    {
                        Name = "Servicio de Temperatura",
                        Endpoints = new[]
                        {
                            new { Path = "/api/Temperatura/{ciudad}", Description = "Consulta temperatura de una ciudad" },
                            new { Path = "/api/Temperatura/multiple/{ciudades}", Description = "Consulta temperatura de múltiples ciudades" }
                        },
                        HealthCheck = "http://localhost:5050/health",
                        HealthUI = "http://localhost:5050/healthchecks-ui",
                        RequiresAuth = true
                    }
                },
                Authentication = new
                {
                    Type = "JWT Bearer Token",
                    Header = "Authorization: Bearer {token}",
                    Required = true
                }
            };

            return Ok(info);
        }

        /// <summary>
        /// Verifica el estado de conectividad con los microservicios
        /// </summary>
        /// <returns>Estado de conectividad de cada microservicio</returns>
        /// <response code="200">Estado de los microservicios</response>
        [HttpGet("status")]
        [Authorize]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetServicesStatus()
        {
            var httpClient = new HttpClient();
            httpClient.Timeout = TimeSpan.FromSeconds(5);

            var dolarServiceUrl = _configuration["MicroservicesUrls:DolarService"] ?? "http://localhost:5104";
            var temperaturaServiceUrl = _configuration["MicroservicesUrls:TemperaturaService"] ?? "http://localhost:5050";

            var status = new
            {
                Timestamp = DateTime.UtcNow,
                Services = new
                {
                    DolarService = await CheckServiceHealth(httpClient, $"{dolarServiceUrl}/health", "DolarService"),
                    TemperaturaService = await CheckServiceHealth(httpClient, $"{temperaturaServiceUrl}/health", "TemperaturaService")
                }
            };

            return Ok(status);
        }

        private async Task<object> CheckServiceHealth(HttpClient httpClient, string healthUrl, string serviceName)
        {
            try
            {
                var response = await httpClient.GetAsync(healthUrl);
                var content = await response.Content.ReadAsStringAsync();
                
                return new
                {
                    Name = serviceName,
                    Status = response.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                    StatusCode = (int)response.StatusCode,
                    Response = content,
                    Url = healthUrl,
                    CheckedAt = DateTime.UtcNow
                };
            }
            catch (Exception ex)
            {
                return new
                {
                    Name = serviceName,
                    Status = "Error",
                    StatusCode = 0,
                    Response = ex.Message,
                    Url = healthUrl,
                    CheckedAt = DateTime.UtcNow
                };
            }
        }
    }
}
