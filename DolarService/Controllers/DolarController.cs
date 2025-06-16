using DolarService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DolarService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DolarController : ControllerBase
    {
        private readonly ILogger<DolarController> _logger;

        public DolarController(ILogger<DolarController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetDolarPrice()
        {
            try
            {
                // Obtener el payload del token JWT
                var username = User.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
                var date = DateTime.Parse(User.FindFirst("Date")?.Value ?? DateTime.Now.ToString());

                _logger.LogInformation($"Consulta de precio del dólar por usuario: {username}, fecha: {date}");                // En un caso real, aquí se consultaría una API externa o base de datos
                // Para este ejemplo, generamos un valor aleatorio entre 18 y 20
                var random = new Random();
                var dolarValue = (decimal)Math.Round(18 + (random.NextDouble() * 2), 2);

                var response = new DolarResponse
                {
                    Valor = dolarValue,
                    FechaConsulta = DateTime.Now,
                    Usuario = username
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el precio del dólar");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        [HttpGet("public")]
        [AllowAnonymous]
        public IActionResult GetDolarPricePublic()
        {
            try
            {
                _logger.LogInformation("Consulta pública del precio del dólar");

                // Generar un valor aleatorio entre 18 y 20
                var random = new Random();
                var dolarValue = (decimal)Math.Round(18 + (random.NextDouble() * 2), 2);

                var response = new DolarResponse
                {
                    Valor = dolarValue,
                    FechaConsulta = DateTime.Now,
                    Usuario = "Public"
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el precio del dólar");
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}