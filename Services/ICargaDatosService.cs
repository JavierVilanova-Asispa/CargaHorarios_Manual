using CargadorHorario.Web.Models;

namespace CargadorHorario.Web.Services
{
    public interface ICargaDatosService
    {
        Task<string> CargarDatosTotal();
        Task<string> CargarDatosPorDistrito(string distrito);
        Task<string> CargarDatosPorCoordinadora(string coordinadora);
        Task<List<CoordinadoraItem>> ObtenerCoordinadorasAsync();
    }
}
