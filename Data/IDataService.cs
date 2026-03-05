using System.Data;

namespace CargadorHorario.Web.Data
{
    public interface IDataService
    {
        Task<DataTable> EjecutarConsultaAsync(string consulta, Dictionary<string, object>? parametros = null);
        Task<DataTable> EjecutarProcedimientoAsync(string nombreProcedimiento, Dictionary<string, object>? parametros = null);
        Task<int> EjecutarComandoAsync(string comando, Dictionary<string, object>? parametros = null);
        Task<object?> EjecutarEscalarAsync(string consulta, Dictionary<string, object>? parametros = null);
    }
}
