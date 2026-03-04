using CargaDatos.Web.Models;
using System.Data;

namespace CargaDatos.Web.Services
{
    public interface ICargaDatosService
    {
        Task<DataTable> ObtenerFechasNoCargadasAsync(int numSemanasCargar);
        Task<string> CargarDatosTotal(DateTime fecha, int numIteraciones);
        Task<string> CargarDatosPorDistrito(DateTime fecha, int numIteraciones, string distrito);
        Task<string> CargarDatosPorCoordinadora(DateTime fecha, int numIteraciones, string coordinadora);
        Task<List<CoordinadoraItem>> ObtenerCoordinadorasAsync();
    }
}
