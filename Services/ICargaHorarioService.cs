using CargadorHorario.Web.Models;

namespace CargadorHorario.Web.Services
{
    public interface ICargaHorarioService
    {
        Task<string> GenerarCargaHorarioAsync(CargaHorarioViewModel model);
        Task<List<FinanciacionItem>> ObtenerFinanciacionesAsync();
    }
}
