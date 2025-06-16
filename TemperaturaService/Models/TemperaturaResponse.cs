namespace TemperaturaService.Models
{
    public class TemperaturaResponse
    {
        public decimal Temperatura { get; set; }
        public string Ciudad { get; set; } = string.Empty;
        public DateTime FechaConsulta { get; set; }
        public string Usuario { get; set; } = string.Empty;
    }
}