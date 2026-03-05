using Microsoft.AspNetCore.Mvc;
using CargadorHorario.Web.Models;
using CargadorHorario.Web.Services;

namespace CargadorHorario.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICargaHorarioService _cargaHorarioService;

        public HomeController(ILogger<HomeController> logger, ICargaHorarioService cargaHorarioService)
        {
            _logger = logger;
            _cargaHorarioService = cargaHorarioService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new CargaHorarioViewModel
            {
                Financiaciones = await _cargaHorarioService.ObtenerFinanciacionesAsync(),
                Mes = DateTime.Now.Month,
                Anio = DateTime.Now.Year,
                Mensaje = "NO SE PUDO REALIZAR LA CARGA DE HORARIO DE LA FECHA: 0.00.00"
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Generar(CargaHorarioViewModel model)
        {
            try
            {
                var resultado = await _cargaHorarioService.GenerarCargaHorarioAsync(model);
                
                model.Mensaje = resultado;
                model.EsError = false;
                model.Financiaciones = await _cargaHorarioService.ObtenerFinanciacionesAsync();
                
                return View("Index", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al generar carga de horario");
                
                model.Mensaje = $"Error al generar la carga: {ex.Message}";
                model.EsError = true;
                model.Financiaciones = await _cargaHorarioService.ObtenerFinanciacionesAsync();
                
                return View("Index", model);
            }
        }

        [HttpPost]
        public IActionResult Salir()
        {
            // Redirigir a una página de cierre o simplemente recargar
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult CambiarMes(int mes, int anio)
        {
            // Devuelve el calendario para el mes seleccionado
            var primerDia = new DateTime(anio, mes, 1);
            var ultimoDia = new DateTime(anio, mes, DateTime.DaysInMonth(anio, mes));
            
            return Json(new
            {
                mes = mes,
                anio = anio,
                primerDia = primerDia.Day,
                diasEnMes = DateTime.DaysInMonth(anio, mes)
            });
        }
    }
}
