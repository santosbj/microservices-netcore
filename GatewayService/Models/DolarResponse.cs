namespace GatewayService.Models
{
    /// <summary>
    /// Respuesta del servicio de dólar
    /// </summary>
    public class DolarResponse
    {
        /// <summary>
        /// Valor del dólar en pesos
        /// </summary>
        public decimal Valor { get; set; }
        
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
