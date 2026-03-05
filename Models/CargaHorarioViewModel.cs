namespace CargadorHorario.Web.Models
{
    public class CargaHorarioViewModel
    {
        public TiposCarga TipoCarga { get; set; } = TiposCarga.Coordinadora;
        
        // Financiación
        public string? FinanciacionSeleccionada { get; set; }
        public List<FinanciacionItem> Financiaciones { get; set; } = new();
        
        // Código de programa
        public string? CodPrograma { get; set; }
        
        // Parámetros de calendario
        public int Mes { get; set; } = DateTime.Now.Month;
        public int Anio { get; set; } = DateTime.Now.Year;
        public int? NumSemanaGeneral { get; set; }
        public int? PrimerDiaSemanaGeneral { get; set; }
        public int? NumSemanasAGenerar { get; set; }
        public string? FechaDia { get; set; }
        
        // ExpAuxiliar
        public string? ExpAuxiliar { get; set; }
        
        // Checkboxes
        public bool ActualizarTablaDiaSemana { get; set; }
        public bool ProyeccionSeparada { get; set; }
        
        // Mensaje de resultado
        public string? Mensaje { get; set; }
        public bool EsError { get; set; }
    }

    public class FinanciacionItem
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
    }

    public enum TiposCarga
    {
        Coordinadora = 1,
        Distrito = 2,
        Todos = 3
    }
}
