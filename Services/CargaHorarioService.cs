using CargadorHorario.Web.Data;
using CargadorHorario.Web.Models;
using System.Data;

namespace CargadorHorario.Web.Services
{
    public class CargaHorarioService : ICargaHorarioService
    {
        private readonly ILogger<CargaHorarioService> _logger;
        private readonly IDataService _dataService;

        public CargaHorarioService(ILogger<CargaHorarioService> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        public async Task<string> GenerarCargaHorarioAsync(CargaHorarioViewModel model)
        {
            _logger.LogInformation("Iniciando generación de carga de horario...");
            _logger.LogInformation("Tipo de carga: {TipoCarga}", model.TipoCarga);
            _logger.LogInformation("Financiación: {Financiacion}", model.FinanciacionSeleccionada);
            _logger.LogInformation("Código Programa: {CodPrograma}", model.CodPrograma);
            _logger.LogInformation("Mes/Año: {Mes}/{Anio}", model.Mes, model.Anio);
            
            try
            {
                // TODO: AQUÍ VA TU LÓGICA DE GENERACIÓN
                // Puedes acceder a todos los parámetros del modelo:
                // - model.TipoCarga (Coordinadora, Distrito, Todos)
                // - model.FinanciacionSeleccionada
                // - model.CodPrograma
                // - model.Anio
                // - model.Mes
                // - model.NumSemanaGeneral
                // - model.PrimerDiaSemanaGeneral
                // - model.NumSemanasAGenerar
                // - model.FechaDia
                // - model.ExpAuxiliar
                // - model.ActualizarTablaDiaSemana
                // - model.ProyeccionSeparada
                
                // Ejemplo de llamada a procedimiento almacenado:
                /*
                var parametros = new Dictionary<string, object>
                {
                    { "@TipoCarga", (int)model.TipoCarga },
                    { "@Financiacion", model.FinanciacionSeleccionada ?? string.Empty },
                    { "@CodPrograma", model.CodPrograma ?? string.Empty },
                    { "@Anio", model.Anio },
                    { "@Mes", model.Mes },
                    { "@NumSemana", model.NumSemanaGeneral ?? 0 },
                    { "@ActualizarTabla", model.ActualizarTablaDiaSemana },
                    { "@ProyeccionSeparada", model.ProyeccionSeparada }
                };
                
                var resultado = await _dataService.EjecutarProcedimientoAsync("sp_GenerarCargaHorario", parametros);
                */
                
                await Task.CompletedTask; // Remover cuando implementes la lógica
                
                _logger.LogInformation("Generación de carga completada exitosamente");
                return "Carga de horario generada exitosamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar la carga de horario");
                throw;
            }
        }

        public async Task<List<FinanciacionItem>> ObtenerFinanciacionesAsync()
        {
            _logger.LogInformation("Obteniendo lista de financiaciones...");

            try
            {
                // TODO: Reemplaza esta consulta con la real de tu base de datos
                var consulta = @"
                    SELECT CodigoFinanciacion AS codigo, 
                           NombreFinanciacion AS nombre 
                    FROM Financiaciones
                    ORDER BY NombreFinanciacion";

                var resultado = await _dataService.EjecutarConsultaAsync(consulta);

                var financiaciones = new List<FinanciacionItem>();
                
                foreach (DataRow row in resultado.Rows)
                {
                    financiaciones.Add(new FinanciacionItem
                    {
                        Codigo = row["codigo"].ToString() ?? string.Empty,
                        Nombre = row["nombre"].ToString() ?? string.Empty
                    });
                }

                _logger.LogInformation("Se obtuvieron {Total} financiaciones", financiaciones.Count);
                return financiaciones;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error al obtener financiaciones, devolviendo lista de ejemplo");
                
                // Lista de ejemplo si no hay tabla en la BD
                return new List<FinanciacionItem>
                {
                    new FinanciacionItem { Codigo = "FIN001", Nombre = "Financiación A" },
                    new FinanciacionItem { Codigo = "FIN002", Nombre = "Financiación B" },
                    new FinanciacionItem { Codigo = "FIN003", Nombre = "Financiación C" }
                };
            }
        }
    }
}
