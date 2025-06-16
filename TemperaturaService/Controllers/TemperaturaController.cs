using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TemperaturaService.Models;

namespace TemperaturaService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TemperaturaController : ControllerBase
    {
        private readonly ILogger<TemperaturaController> _logger;

        public TemperaturaController(ILogger<TemperaturaController> logger)
        {
            _logger = logger;
        }

        [HttpGet("{ciudad}")]
        public IActionResult GetTemperatura(string ciudad)
        {
            try
            {
                // Obtener el payload del token JWT
                var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
                var date = DateTime.Parse(User.FindFirst("Date")?.Value ?? DateTime.Now.ToString());

                _logger.LogInformation($"Consulta de temperatura para la ciudad: {ciudad} por usuario: {username}, fecha: {date}");

                if (string.IsNullOrWhiteSpace(ciudad))
                {
                    return BadRequest("Debe proporcionar una ciudad válida");
                }                // En un caso real, aquí se consultaría una API externa de clima
                // Para este ejemplo, generamos un valor aleatorio entre 0 y 40 grados
                var random = new Random();
                var temperatura = (decimal)Math.Round(random.NextDouble() * 40, 1);

                var response = new TemperaturaResponse
                {
                    Temperatura = temperatura,
                    Ciudad = ciudad,
                    FechaConsulta = DateTime.Now,
                    Usuario = username
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la temperatura para la ciudad: {ciudad}");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("public/{ciudad}")]
        [AllowAnonymous]
        public IActionResult GetTemperaturaPublic(string ciudad)
        {
            try
            {
                _logger.LogInformation($"Consulta pública de temperatura para la ciudad: {ciudad}");

                if (string.IsNullOrWhiteSpace(ciudad))
                {
                    return BadRequest("Debe proporcionar una ciudad válida");
                }

                // Generar un valor aleatorio entre 0 y 40 grados
                var random = new Random();
                var temperatura = (decimal)Math.Round(random.NextDouble() * 40, 1);

                var response = new TemperaturaResponse
                {
                    Temperatura = temperatura,
                    Ciudad = ciudad,
                    FechaConsulta = DateTime.Now,
                    Usuario = "Public"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener la temperatura para la ciudad: {ciudad}");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}