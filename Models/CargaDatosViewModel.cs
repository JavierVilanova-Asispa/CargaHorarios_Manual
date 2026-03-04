namespace CargaDatos.Web.Models
{
    public class CargaDatosViewModel
    {
        public TipoCarga TipoCarga { get; set; }
        public string? DistritoSeleccionado { get; set; }
        public string? CoordinadoraSeleccionada { get; set; }
        public List<CoordinadoraItem> Coordinadoras { get; set; } = new();
        public string? Mensaje { get; set; }
        public bool EsError { get; set; }
    }

    public class CoordinadoraItem
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
    }

    public enum TipoCarga
    {
        Total = 1,
        PorDistrito = 2,
        PorCoordinadora = 3
    }
}
