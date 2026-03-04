using Microsoft.AspNetCore.Mvc;
using CargaDatos.Web.Models;
using CargaDatos.Web.Services;
using System.Data;

namespace CargaDatos.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICargaDatosService _cargaDatosService;

        private const int NUM_SEMANAS = 2;
        private static bool _yaCargado = false;

        public HomeController(ILogger<HomeController> logger, ICargaDatosService cargaDatosService)
        {
            _logger = logger;
            _cargaDatosService = cargaDatosService;
        }

        public async Task<IActionResult> Index()
        {
            var model = new CargaDatosViewModel
            {
                Coordinadoras = await _cargaDatosService.ObtenerCoordinadorasAsync()
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EjecutarCarga(CargaDatosViewModel model)
        {
            try
            {
                if (!_yaCargado)
                {
                    _yaCargado = true;
                    DataTable dt = await _cargaDatosService.ObtenerFechasNoCargadasAsync(NUM_SEMANAS);
                    int numIteraciones = 0;
                    string resultado = string.Empty;

                    foreach (DataRow row in dt.Rows)
                    {
                        numIteraciones++;
                        DateTime fecha = Convert.ToDateTime(row["fecha"].ToString());

                        switch (model.TipoCarga)
                        {
                            case TipoCarga.Total:
                                resultado = await _cargaDatosService.CargarDatosTotal(fecha, numIteraciones);
                                break;

                            case TipoCarga.PorDistrito:
                                if (string.IsNullOrWhiteSpace(model.DistritoSeleccionado))
                                {
                                    model.Mensaje = "Por favor, introduce el nombre del distrito";
                                    model.EsError = true;
                                    model.Coordinadoras = await _cargaDatosService.ObtenerCoordinadorasAsync();
                                    return View("Index", model);
                                }
                                resultado = await _cargaDatosService.CargarDatosPorDistrito(fecha, numIteraciones, model.DistritoSeleccionado);
                                break;

                            case TipoCarga.PorCoordinadora:
                                if (string.IsNullOrEmpty(model.CoordinadoraSeleccionada))
                                {
                                    model.Mensaje = "Por favor, selecciona una coordinadora";
                                    model.EsError = true;
                                    model.Coordinadoras = await _cargaDatosService.ObtenerCoordinadorasAsync();
                                    return View("Index", model);
                                }
                                resultado = await _cargaDatosService.CargarDatosPorCoordinadora(fecha, numIteraciones, model.CoordinadoraSeleccionada);
                                break;
                        }
                    }

                    model.Mensaje = resultado;
                    model.EsError = false;
                    model.Coordinadoras = await _cargaDatosService.ObtenerCoordinadorasAsync();
                    return View("Index", model);
                }
                else
                {
                    Exception ex = new("La carga ya ha sido realizada");
                    _logger.LogError(ex, ex.Message);
                    model.Mensaje = $"Error al ejecutar la carga: {ex.Message}";
                    model.EsError = true;
                    model.Coordinadoras = await _cargaDatosService.ObtenerCoordinadorasAsync();
                    return View("Index", model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al ejecutar la carga de datos");
                model.Mensaje = $"Error al ejecutar la carga: {ex.Message}";
                model.EsError = true;
                model.Coordinadoras = await _cargaDatosService.ObtenerCoordinadorasAsync();
                return View("Index", model);
            }
        }
    }
}
