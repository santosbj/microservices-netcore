namespace GatewayService.Models
{
    /// <summary>
    /// Respuesta del servicio de temperatura
    /// </summary>
    public class TemperaturaResponse
    {
        /// <summary>
        /// Temperatura en grados Celsius
        /// </summary>
        public decimal Temperatura { get; set; }
        
        /// <summary>
        /// Ciudad consultada
        /// </summary>
        public string Ciudad { get; set; } = string.Empty;
        
        /// <summary>
        /// Fecha y hora de la consulta
        /// </summary>
        public DateTime FechaConsulta { get; set; }
        
        /// <summary>
        /// Usuario que realizó la consulta
        /// </summary>
        public string Usuario { get; set; } = string.Empty;
    }
}
