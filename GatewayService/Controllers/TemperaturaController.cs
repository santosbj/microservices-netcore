using GatewayService.Models;
using GatewayService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GatewayService.Controllers
{
    /// <summary>
    /// Controlador para operaciones relacionadas con temperatura
    /// Actúa como proxy hacia el microservicio TemperaturaService
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TemperaturaController : ControllerBase
    {
        private readonly IMicroservicesService _microservicesService;
        private readonly ILogger<TemperaturaController> _logger;

        public TemperaturaController(IMicroservicesService microservicesService, ILogger<TemperaturaController> logger)
        {
            _microservicesService = microservicesService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene la temperatura actual de una ciudad específica
        /// </summary>
        /// <param name="ciudad">Nombre de la ciudad a consultar</param>
        /// <returns>Información de temperatura de la ciudad</returns>
        /// <response code="200">Retorna la temperatura de la ciudad exitosamente</response>
        /// <response code="400">Solicitud inválida - Ciudad requerida</response>
        /// <response code="401">No autorizado - Token JWT requerido</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("{ciudad}")]
        [ProducesResponseType(typeof(TemperaturaResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTemperatura(string ciudad)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ciudad))
                {
                    return BadRequest("El parámetro 'ciudad' es requerido");
                }

                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader))
                {
                    return Unauthorized("Token de autorización requerido");
                }

                var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Usuario desconocido";
                _logger.LogInformation("Usuario {Username} consultando temperatura de {Ciudad}", username, ciudad);

                var result = await _microservicesService.GetTemperaturaAsync(ciudad, authHeader);
                
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return StatusCode(500, "Error al obtener información de temperatura");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar solicitud de temperatura para ciudad: {Ciudad}", ciudad);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene la temperatura de múltiples ciudades
        /// </summary>
        /// <param name="ciudades">Lista de ciudades separadas por comas</param>
        /// <returns>Lista de temperaturas de las ciudades solicitadas</returns>
        /// <response code="200">Retorna las temperaturas de las ciudades exitosamente</response>
        /// <response code="400">Solicitud inválida - Al menos una ciudad requerida</response>
        /// <response code="401">No autorizado - Token JWT requerido</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("multiple/{ciudades}")]
        [ProducesResponseType(typeof(List<TemperaturaResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTemperaturaMultiple(string ciudades)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ciudades))
                {
                    return BadRequest("El parámetro 'ciudades' es requerido");
                }

                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader))
                {
                    return Unauthorized("Token de autorización requerido");
                }

                var listaCiudades = ciudades.Split(',', StringSplitOptions.RemoveEmptyEntries)
                                          .Select(c => c.Trim())
                                          .Where(c => !string.IsNullOrEmpty(c))
                                          .ToList();

                if (!listaCiudades.Any())
                {
                    return BadRequest("Al menos una ciudad válida es requerida");
                }

                var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Usuario desconocido";
                _logger.LogInformation("Usuario {Username} consultando temperatura de múltiples ciudades: {Ciudades}", 
                    username, string.Join(", ", listaCiudades));

                var tasks = listaCiudades.Select(ciudad => 
                    _microservicesService.GetTemperaturaAsync(ciudad, authHeader));
                
                var resultados = await Task.WhenAll(tasks);
                var resultadosValidos = resultados.Where(r => r != null).ToList();

                return Ok(resultadosValidos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar solicitud de temperatura múltiple");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene la temperatura de una ciudad específica (endpoint público para testing)
        /// </summary>
        /// <param name="ciudad">Nombre de la ciudad a consultar</param>
        /// <returns>Información de temperatura de la ciudad</returns>
        /// <response code="200">Retorna la temperatura de la ciudad exitosamente</response>
        /// <response code="400">Solicitud inválida - Ciudad requerida</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet("public/{ciudad}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(TemperaturaResponse), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetTemperaturaPublic(string ciudad)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(ciudad))
                {
                    return BadRequest("El parámetro 'ciudad' es requerido");
                }

                _logger.LogInformation("Consulta pública de temperatura para ciudad: {Ciudad} a través del Gateway", ciudad);

                var result = await _microservicesService.GetTemperaturaPublicAsync(ciudad);
                
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return StatusCode(500, "Error al obtener información de temperatura");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar solicitud pública de temperatura para ciudad: {Ciudad}", ciudad);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
