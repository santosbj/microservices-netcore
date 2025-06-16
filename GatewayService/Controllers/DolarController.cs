using GatewayService.Models;
using GatewayService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GatewayService.Controllers
{
    /// <summary>
    /// Controlador para operaciones relacionadas con el dólar
    /// Actúa como proxy hacia el microservicio DolarService
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DolarController : ControllerBase
    {
        private readonly IMicroservicesService _microservicesService;
        private readonly ILogger<DolarController> _logger;

        public DolarController(IMicroservicesService microservicesService, ILogger<DolarController> logger)
        {
            _microservicesService = microservicesService;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene el valor actual del dólar
        /// </summary>
        /// <returns>Información del precio del dólar</returns>
        /// <response code="200">Retorna el precio del dólar exitosamente</response>
        /// <response code="401">No autorizado - Token JWT requerido</response>
        /// <response code="500">Error interno del servidor</response>
        [HttpGet]
        [ProducesResponseType(typeof(DolarResponse), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> GetDolar()
        {
            try
            {
                var authHeader = Request.Headers["Authorization"].FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader))
                {
                    return Unauthorized("Token de autorización requerido");
                }

                var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Usuario desconocido";
                _logger.LogInformation("Usuario {Username} consultando precio del dólar", username);

                var result = await _microservicesService.GetDolarAsync(authHeader);
                
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return StatusCode(500, "Error al obtener información del dólar");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar solicitud de dólar");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        /// <summary>
        /// Obtiene el valor actual del dólar (endpoint público para testing)
        /// </summary>
        /// <returns>Información del precio del dólar</returns>
        [HttpGet("public")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDolarPublic()
        {
            try
            {
                _logger.LogInformation("Consulta pública del precio del dólar a través del Gateway");

                var result = await _microservicesService.GetDolarPublicAsync();
                
                if (result != null)
                {
                    return Ok(result);
                }
                else
                {
                    return StatusCode(500, "Error al obtener información del dólar");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar solicitud pública de dólar");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
