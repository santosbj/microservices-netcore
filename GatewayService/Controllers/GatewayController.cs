using GatewayService.Models;
using GatewayService.Services;
using Microsoft.AspNetCore.Mvc;

namespace GatewayService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GatewayController : ControllerBase
    {
        private readonly ILogger<GatewayController> _logger;
        private readonly IJwtService _jwtService;

        public GatewayController(ILogger<GatewayController> logger, IJwtService jwtService)
        {
            _logger = logger;
            _jwtService = jwtService;
        }

        [HttpPost("token")]
        public IActionResult GenerateToken([FromBody] UserPayload payload)
        {
            try
            {
                if (payload == null || string.IsNullOrEmpty(payload.Username))
                {
                    return BadRequest("El nombre de usuario es requerido");
                }

                // Si no se proporciona una fecha, usar la fecha actual
                if (payload.Date == default)
                {
                    payload.Date = DateTime.Now;
                }

                var token = _jwtService.GenerateToken(payload);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar el token");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // Este endpoint es solo para pruebas, ya que el proxy se encargará de redirigir las solicitudes
        [HttpGet("info")]
        public IActionResult GetInfo()
        {
            return Ok(new { Message = "Gateway API funcionando correctamente", Timestamp = DateTime.Now });
        }
    }
}