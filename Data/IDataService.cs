using System.Data;

namespace CargaDatos.Web.Data
{
    public interface IDataService
    {
        /// <summary>
        /// Ejecuta una consulta SELECT y devuelve un DataTable con los resultados
        /// </summary>
        Task<DataTable> EjecutarConsultaAsync(string consulta, Dictionary<string, object>? parametros = null);

        /// <summary>
        /// Ejecuta un procedimiento almacenado y devuelve un DataTable con los resultados
        /// </summary>
        Task<DataTable> EjecutarProcedimientoAsync(string nombreProcedimiento, Dictionary<string, object>? parametros = null);

        /// <summary>
        /// Ejecuta un comando SQL (INSERT, UPDATE, DELETE) y devuelve el número de filas afectadas
        /// </summary>
        Task<int> EjecutarComandoAsync(string comando, Dictionary<string, object>? parametros = null);

        /// <summary>
        /// Ejecuta una consulta y devuelve un valor escalar (primera columna de la primera fila)
        /// </summary>
        Task<object?> EjecutarEscalarAsync(string consulta, Dictionary<string, object>? parametros = null);
    }
}
