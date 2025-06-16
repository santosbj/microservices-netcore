namespace DolarService.Models
{
    public class DolarResponse
    {
        public decimal Valor { get; set; }
        public DateTime FechaConsulta { get; set; }
        public string Usuario { get; set; } = string.Empty;
    }
}