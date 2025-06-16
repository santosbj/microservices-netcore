using GatewayService.Services;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace GatewayService.Middleware
{    public class JwtForwardingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtForwardingMiddleware> _logger;

        public JwtForwardingMiddleware(RequestDelegate next, ILogger<JwtForwardingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower();            // No aplicar el middleware a las rutas públicas
            if (path != null && (
                path.Contains("/health") || 
                path.Contains("/swagger") || 
                path.Contains("/api/info") ||
                path.Contains("/healthchecks-ui") ||
                path.Contains("/healthchecks-api") ||
                path == "/" ||
                path.Contains("/api/gateway/token") || 
                path.Contains("/api/gateway/info")))
            {
                await _next(context);
                return;
            }

            // Obtener el token de autorización
            string? authHeader = context.Request.Headers["Authorization"];
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Token de autorización no proporcionado");
                return;
            }

            string token = authHeader.Substring("Bearer ".Length).Trim();

            try
            {
                // Obtener el servicio JWT del scope actual
                var jwtService = context.RequestServices.GetRequiredService<IJwtService>();
                
                // Continuar con el pipeline
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el middleware de reenvío JWT");
                context.Response.StatusCode = 500;
                await context.Response.WriteAsync("Error interno del servidor");
            }
        }
    }

    // Extensión para agregar el middleware al pipeline
    public static class JwtForwardingMiddlewareExtensions
    {
        public static IApplicationBuilder UseJwtForwarding(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<JwtForwardingMiddleware>();
        }
    }
}