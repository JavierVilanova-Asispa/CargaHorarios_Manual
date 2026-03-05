using CargadorHorario.Web.Models;
using CargadorHorario.Web.Data;
using System.Data;

namespace CargadorHorario.Web.Services
{
    public class CargaDatosService : ICargaDatosService
    {
        private readonly ILogger<CargaDatosService> _logger;
        private readonly IDataService _dataService;

        public CargaDatosService(ILogger<CargaDatosService> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        public async Task<string> CargarDatosTotal()
        {
            _logger.LogInformation("Iniciando carga total de datos...");

            try
            {
                // TODO: Implementar lógica de carga total
                // Ejemplo:
                // var resultado = await _dataService.EjecutarProcedimientoAsync("sp_CargaDatosTotal");

                _logger.LogInformation("Carga total completada");
                return "Carga total de datos completada exitosamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la carga total de datos");
                throw;
            }
        }

        public async Task<string> CargarDatosPorDistrito(string distrito)
        {
            _logger.LogInformation("Iniciando carga de datos por distrito: {Distrito}", distrito);

            try
            {
                // TODO: Implementar lógica de carga por distrito
                // Ejemplo:
                // var parametros = new Dictionary<string, object> { { "@Distrito", distrito } };
                // var resultado = await _dataService.EjecutarProcedimientoAsync("sp_CargaDatosPorDistrito", parametros);

                _logger.LogInformation("Carga por distrito {Distrito} completada", distrito);
                return $"Carga de datos para el distrito '{distrito}' completada exitosamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la carga por distrito: {Distrito}", distrito);
                throw;
            }
        }

        public async Task<string> CargarDatosPorCoordinadora(string coordinadora)
        {
            _logger.LogInformation("Iniciando carga de datos por coordinadora: {Coordinadora}", coordinadora);

            try
            {
                // TODO: Implementar lógica de carga por coordinadora
                // Ejemplo:
                // var parametros = new Dictionary<string, object> { { "@Coordinadora", coordinadora } };
                // var resultado = await _dataService.EjecutarProcedimientoAsync("sp_CargaDatosPorCoordinadora", parametros);

                _logger.LogInformation("Carga por coordinadora {Coordinadora} completada", coordinadora);
                return $"Carga de datos para la coordinadora '{coordinadora}' completada exitosamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la carga por coordinadora: {Coordinadora}", coordinadora);
                throw;
            }
        }

        public async Task<List<CoordinadoraItem>> ObtenerCoordinadorasAsync()
        {
            _logger.LogInformation("Obteniendo lista de coordinadoras...");

            try
            {
                var consulta = @"
                    SELECT codigoCoordinador AS codigo, 
                           ApellidosCoordinador + ', ' + NombreCoordinador AS nombre 
                    FROM Coordinadores
                    ORDER BY ApellidosCoordinador, NombreCoordinador";

                var resultado = await _dataService.EjecutarConsultaAsync(consulta);

                var coordinadoras = resultado.AsEnumerable()
                    .Select(row => new CoordinadoraItem
                    {
                        Codigo = row["codigo"].ToString() ?? string.Empty,
                        Nombre = row["nombre"].ToString() ?? string.Empty
                    })
                    .ToList();

                _logger.LogInformation("Se obtuvieron {Total} coordinadoras", coordinadoras.Count);
                return coordinadoras;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de coordinadoras");
                throw;
            }
        }
    }
}
