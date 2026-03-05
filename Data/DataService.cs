using Microsoft.Data.SqlClient;
using System.Data;

namespace CargadorHorario.Web.Data
{
    public class DataService : IDataService
    {
        private readonly string _connectionString;
        private readonly ILogger<DataService> _logger;

        public DataService(IConfiguration configuration, ILogger<DataService> logger)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? throw new InvalidOperationException("La cadena de conexión 'DefaultConnection' no está configurada");
            _logger = logger;
        }

        public async Task<DataTable> EjecutarConsultaAsync(string consulta, Dictionary<string, object>? parametros = null)
        {
            var dataTable = new DataTable();

            try
            {
                _logger.LogInformation("Ejecutando consulta: {Consulta}", consulta);

                using var conexion = new SqlConnection(_connectionString);
                await conexion.OpenAsync();

                using var comando = new SqlCommand(consulta, conexion);
                
                if (parametros != null)
                {
                    foreach (var parametro in parametros)
                    {
                        comando.Parameters.AddWithValue(parametro.Key, parametro.Value ?? DBNull.Value);
                    }
                }

                using var adapter = new SqlDataAdapter(comando);
                adapter.Fill(dataTable);

                _logger.LogInformation("Consulta ejecutada exitosamente. Filas devueltas: {Filas}", dataTable.Rows.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ejecutar la consulta: {Consulta}", consulta);
                throw;
            }

            return dataTable;
        }

        public async Task<DataTable> EjecutarProcedimientoAsync(string nombreProcedimiento, Dictionary<string, object>? parametros = null)
        {
            var dataTable = new DataTable();

            try
            {
                _logger.LogInformation("Ejecutando procedimiento almacenado: {Procedimiento}", nombreProcedimiento);

                using var conexion = new SqlConnection(_connectionString);
                await conexion.OpenAsync();

                using var comando = new SqlCommand(nombreProcedimiento, conexion)
                {
                    CommandType = CommandType.StoredProcedure
                };

                if (parametros != null)
                {
                    foreach (var parametro in parametros)
                    {
                        comando.Parameters.AddWithValue(parametro.Key, parametro.Value ?? DBNull.Value);
                    }
                }

                using var adapter = new SqlDataAdapter(comando);
                adapter.Fill(dataTable);

                _logger.LogInformation("Procedimiento ejecutado exitosamente. Filas devueltas: {Filas}", dataTable.Rows.Count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ejecutar el procedimiento almacenado: {Procedimiento}", nombreProcedimiento);
                throw;
            }

            return dataTable;
        }

        public async Task<int> EjecutarComandoAsync(string comando, Dictionary<string, object>? parametros = null)
        {
            try
            {
                _logger.LogInformation("Ejecutando comando: {Comando}", comando);

                using var conexion = new SqlConnection(_connectionString);
                await conexion.OpenAsync();

                using var sqlCommand = new SqlCommand(comando, conexion);

                if (parametros != null)
                {
                    foreach (var parametro in parametros)
                    {
                        sqlCommand.Parameters.AddWithValue(parametro.Key, parametro.Value ?? DBNull.Value);
                    }
                }

                var filasAfectadas = await sqlCommand.ExecuteNonQueryAsync();

                _logger.LogInformation("Comando ejecutado exitosamente. Filas afectadas: {Filas}", filasAfectadas);

                return filasAfectadas;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ejecutar el comando: {Comando}", comando);
                throw;
            }
        }

        public async Task<object?> EjecutarEscalarAsync(string consulta, Dictionary<string, object>? parametros = null)
        {
            try
            {
                _logger.LogInformation("Ejecutando consulta escalar: {Consulta}", consulta);

                using var conexion = new SqlConnection(_connectionString);
                await conexion.OpenAsync();

                using var comando = new SqlCommand(consulta, conexion);

                if (parametros != null)
                {
                    foreach (var parametro in parametros)
                    {
                        comando.Parameters.AddWithValue(parametro.Key, parametro.Value ?? DBNull.Value);
                    }
                }

                var resultado = await comando.ExecuteScalarAsync();

                _logger.LogInformation("Consulta escalar ejecutada exitosamente");

                return resultado;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ejecutar la consulta escalar: {Consulta}", consulta);
                throw;
            }
        }
    }
}
