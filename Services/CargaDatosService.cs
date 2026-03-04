using CargaDatos.Web.Data;
using CargaDatos.Web.Models;
using Microsoft.AspNetCore.Http;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Security.Cryptography;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CargaDatos.Web.Services
{
    public class CargaDatosService : ICargaDatosService
    {
        private readonly ILogger<CargaDatosService> _logger;
        private readonly IDataService _dataService;
        private const string CTE_SEPARADOR = "*";

        public CargaDatosService(ILogger<CargaDatosService> logger, IDataService dataService)
        {
            _logger = logger;
            _dataService = dataService;
        }

        public async Task<DataTable> ObtenerFechasNoCargadasAsync(int numSemanasCargar)
        {
            _logger.LogInformation("Iniciando obtención de fechas no cargadas...");

            var consulta = @$"
                SELECT Top {numSemanasCargar} numSemana, Fecha 
                FROM TBL_DiaSemana
                WHERE Cargado = 0 
                ORDER BY Fecha ASC";

            return await _dataService.EjecutarConsultaAsync(consulta);
        }

        public async Task<string> CargarDatosTotal(DateTime dia1Semana, int numIteraciones)
        {
            _logger.LogInformation("Iniciando carga total de datos...");

            try
            {
                List<string> diasFestivos = new List<string>();
                DateTime ultimoDomingo = dia1Semana.AddDays(6);

                string sql = $@"
                    DELETE From [Horario de Auxiliares]
                    WHERE DiaHorario >= {dia1Semana}
                        AND DiaHorario <= {ultimoDomingo}";
                var resultado = await _dataService.EjecutarComandoAsync(sql);


                sql = CrearSQLCrearFichasSemana(ultimoDomingo);
                var dtFichasSemana = await _dataService.EjecutarConsultaAsync(sql);

                CargarDiasFestivos(dia1Semana, ultimoDomingo, diasFestivos);

                if (diasFestivos.Count == 0)
                {
                    string err = "Error en la carga total de datos: no se han encontrado días festivos";
                    _logger.LogInformation(err);
                    return (err);
                }

                foreach (DataRow row in dtFichasSemana.Rows)
                {
                    await GenerarRegistrosTablaHorario(numIteraciones, dia1Semana, ultimoDomingo, dtFichasSemana, diasFestivos, 0);
                }

                _logger.LogInformation("Carga total completada");
                return "Carga total de datos completada exitosamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la carga total de datos");
                throw;
            }
        }

        public async Task<string> CargarDatosPorDistrito(DateTime dia1Semana, int numIteraciones, string distrito)
        {
            _logger.LogInformation("Iniciando carga de datos por distrito: {Distrito}", distrito);

            if (distrito.Equals("0"))
            {
                Exception ex = new("Error en la carga por distrito: distrito no puede ser 0.");
                _logger.LogError(ex, ex.Message);
                throw ex;
            }

            try
            {
                List<string> diasFestivos = new List<string>();
                DateTime ultimoDomingo = dia1Semana.AddDays(6);

                string sql = $@"
                    DELETE From [Horario de Auxiliares]
                    WHERE DiaHorario >= {dia1Semana}
                        AND DiaHorario <= {ultimoDomingo} 
                        AND EXISTS (SELECT ExpAuxiliar 
                                    FROM Auxiliares 
                                    WHERE expAuxiliar=[Horario de Auxiliares].expAuxiliar 
                                        AND CodigoZona= {distrito})
                ";

                sql = CrearSQLCrearFichasSemana(ultimoDomingo);
                var dtFichasSemana = await _dataService.EjecutarConsultaAsync(sql);

                CargarDiasFestivos(dia1Semana, ultimoDomingo, diasFestivos);

                if (diasFestivos.Count == 0)
                {
                    string err = "Error en la carga total de datos: no se han encontrado días festivos";
                    _logger.LogInformation(err);
                    return (err);
                }

                foreach (DataRow row in dtFichasSemana.Rows)
                {
                    await GenerarRegistrosTablaHorario(numIteraciones, dia1Semana, ultimoDomingo, dtFichasSemana, diasFestivos, 0);
                }

                _logger.LogInformation("Carga por distrito {Distrito} completada", distrito);
                return $"Carga de datos para el distrito '{distrito}' completada exitosamente";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en la carga por distrito: {Distrito}", distrito);
                throw;
            }
        }

        public async Task<string> CargarDatosPorCoordinadora(DateTime dia1Semana, int numIteraciones, string coordinadora)
        {
            _logger.LogInformation("Iniciando carga de datos por coordinadora: {Coordinadora}", coordinadora);

            try
            {
                List<string> diasFestivos = new List<string>();
                DateTime ultimoDomingo = dia1Semana.AddDays(6);

                string sql = $@"
                    DELETE From [Horario de Auxiliares]
                    WHERE DiaHorario >= {dia1Semana}
                    AND DiaHorario <= {ultimoDomingo}
                        AND ExpAuxiliar IN (SELECT ExpAuxiliar 
                                            FROM Auxiliares 
                                            WHERE CodigoCoordinador = {coordinadora})
                        AND EXISTS (SELECT ExpAuxiliar 
                                    FROM Auxiliares 
                                    WHERE expAuxiliar=[Horario de Auxiliares].expAuxiliar 
                                        AND CodigoCoordinador = {coordinadora})
                    ";
                var resultado = await _dataService.EjecutarComandoAsync(sql);

                sql = CrearSQLCrearFichasSemana(ultimoDomingo);
                var dtFichasSemana = await _dataService.EjecutarConsultaAsync(sql);

                CargarDiasFestivos(dia1Semana, ultimoDomingo, diasFestivos);

                if (diasFestivos.Count == 0)
                {
                    string err = "Error en la carga total de datos: no se han encontrado días festivos";
                    _logger.LogInformation(err);
                    return (err);
                }

                foreach (DataRow row in dtFichasSemana.Rows)
                {
                    await GenerarRegistrosTablaHorario(numIteraciones, dia1Semana, ultimoDomingo, dtFichasSemana, diasFestivos, 0);
                }

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

        private static string CrearSQLCrearFichasSemana(DateTime ultimoDomingo, int idDistrito = 0, int idCoordinadora = 0)
        {
            ultimoDomingo = ultimoDomingo.AddDays(1);
            string filtroCoordinadora = "";
            string filtroDistrito = "";

            if (idCoordinadora > 0)
                filtroCoordinadora = $"AND Auxiliares.codigoCoordinador = {idCoordinadora}";
            if (idDistrito > 0)
                filtroDistrito = $"Auxiliares.codigoZona = {idDistrito}";

            string retVal = $@"
                SELECT Auxiliares.ExpAuxiliar, Servicios.InicioLunes, Servicios.Lunes, Servicios.InicioMartes,
                    Servicios.Martes, Servicios.InicioMiercoles, Servicios.Miercoles, Servicios.InicioJueves, Servicios.Jueves,
                    Servicios.InicioViernes, Servicios.Viernes, Servicios.InicioSabado, Servicios.Sabado, Servicios.InicioDomingo,
                    Servicios.Domingo, Usuarios.ExpUsuario, Servicios.CodigoServicio, Usuarios.CodigoDiaFiesta, Servicios.Privado,
                    Servicios.BajaServicio, Servicios.AltaServicio, Servicios.CodigoPeriodo, Servicios.RefCodigoServicio,
                    Usuarios.Telefono, Servicios.ConceptoHoras, Servicios.TP_L, Servicios.TP_M, Servicios.TP_X, Servicios.TP_J, 
                    Servicios.TP_V, Servicios.TP_S, Servicios.TP_D, Servicios_InicioTarde.InicioTarde_L, 
                    Servicios_InicioTarde.InicioTarde_M, Servicios_InicioTarde.InicioTarde_X, Servicios_InicioTarde.InicioTarde_J, 
                    Servicios_InicioTarde.InicioTarde_V, Servicios_InicioTarde.InicioTarde_S, Servicios_InicioTarde.InicioTarde_D, 
                    Usuarios_NoServFestivo.NoPrestarServDiaFestivo, Servicios.NoFichajeEntrada, Servicios.NoFichajeSalida, 
                    Servicios.AC_ExpUsuario2
                FROM Usuarios INNER JOIN Auxiliares
                    INNER JOIN [Servicios por Usuario] Servicios
                        ON Auxiliares.ExpAuxiliar = Servicios.ExpAuxiliar
                        ON Usuarios.ExpUsuario = Servicios.ExpUsuario
                    LEFT JOIN Servicios_InicioTarde
                        ON Servicios_InicioTarde.CodigoServicio=Servicios.CodigoServicio
                    LEFT JOIN Usuarios_NoServFestivo
                        ON Usuarios.expUsuario=Usuarios_NoServFestivo.expUsuario
                WHERE (Servicios.BajaServicio Is Null OR Servicios.BajaServicio >= '{ultimoDomingo}')
                    AND Servicios.AltaServicio <= '{ultimoDomingo}'
                    AND Servicios.RefCodigoServicio = 0
                    {filtroDistrito}
                    {filtroCoordinadora}
                ORDER BY Auxiliares.CodigoZona Asc, Auxiliares.CodigoCoordinador Asc, 
                    Auxiliares.ExpAuxiliar Asc, Servicios.ExpUsuario Asc";
            
            return retVal;
        }

        private async void CargarDiasFestivos(DateTime dia1Semana, DateTime ultimoDomingo, List<string> diasFestivos)
        {
            string sql = $@"
                SELECT DISTINCT codigoDiaFiesta, DiaFiesta
                FROM [Dias de Fiesta]
                WHERE DiaFiesta >= '{dia1Semana}'
                    AND DiaFiesta <= '{ultimoDomingo}'";

            var dtDiasFestivos = await _dataService.EjecutarConsultaAsync(sql);

            foreach (DataRow row in dtDiasFestivos.Rows)
            {
                diasFestivos.Add(row["CodigoDiaFiesta"].ToString() + CTE_SEPARADOR + row["DiaFiesta"].ToString());
            }
        }

        private async Task<int> GenerarRegistrosTablaHorario(int numIteraciones, DateTime dia1Semana, 
            DateTime ultimoDomingo, DataTable dtServ, List<string> diasFestivos, int proyeccion)
        {

            DateTime diaAlta, lunesFechaAlta, diaProc, lunesFechaProc;
            Int64 expAuxiliar, expUsuario, codServicio;
            int tipoPeriodo, regimen, tPublicoDia, inicioTarde, 
                noFichaEntradaDia, noFichaSalidaDia, AC_ExpUsuario2,
                intDiaProc;
            string strNoFichaEntrada, strNoFichaSalida, myId, horaInicio, duracion, sql;
            bool crearFicha, anadirDia, festivo, diaProcIsFiesta;

            diaAlta = Convert.ToDateTime(dtServ.Rows[0]["AltaServicio"]);
            lunesFechaAlta = diaAlta;
            while (!lunesFechaAlta.DayOfWeek.Equals(DayOfWeek.Monday))
            {
                lunesFechaAlta.AddDays(-1);
            }

            tipoPeriodo = Convert.ToInt16(dtServ.Rows[0]["CodigoPeriodo"]);
            expAuxiliar = Convert.ToInt64(dtServ.Rows[0]["expAuxiliar"]);
            expUsuario = Convert.ToInt64(dtServ.Rows[0]["expUsuario"]);
            codServicio = Convert.ToInt64(dtServ.Rows[0]["CodigoServicio"]);
            regimen = Convert.ToInt16(dtServ.Rows[0]["Privado"]);
            strNoFichaEntrada = Convert.ToString(dtServ.Rows[0]["NoFichajeEntrada"] ?? "");
            strNoFichaSalida = Convert.ToString(dtServ.Rows[0]["NoFichajeSalida"] ?? "");
            AC_ExpUsuario2 = Convert.ToInt16(dtServ.Rows[0]["AC_ExpUsuario2"]);

            diaProc = dia1Semana;
            while (diaProc <= ultimoDomingo)
            {
                intDiaProc = Weekday(diaProc);
                lunesFechaProc = diaProc.AddDays(-intDiaProc + 1);

                if (diaAlta <= ultimoDomingo)
                {
                    crearFicha = false;
                    if (tipoPeriodo == 0)
                        crearFicha = true;
                    else
                    {
                        if (tipoPeriodo == 1) // Quincenal
                        {
                            /* Comprueba si la diferencia en semanas, entre el lunes actual y 
                               el lunes de la fecha de creación del servicio es un nº en par. */

                            if (((int)(lunesFechaProc - lunesFechaAlta).TotalDays / 7) % 2 == 0)
                                crearFicha = true;
                        }
                        else
                        {
                            /* Comprueba si la diferencia en semanas, entre el lunes actual 
                               y el lunes de la fecha de creación del servicio es 4. */
                            if (((int)(lunesFechaProc - lunesFechaAlta).TotalDays / 7) % 4 == 0)
                                crearFicha = true;
                        }
                    }

                    if (crearFicha && (diaProc >= diaAlta))
                    {
                        diaProcIsFiesta = EsFestivo(Convert.ToInt16(dtServ.Rows[0]["CodigoDiaFiesta"]), diaProc, diasFestivos);

                        CargarVariablesDiaRegHorario(dtServ, intDiaProc, diaProcIsFiesta, strNoFichaEntrada, strNoFichaSalida,
                            out noFichaEntradaDia, out noFichaSalidaDia, out inicioTarde, out tPublicoDia, out horaInicio, 
                            out duracion);

                        anadirDia = false;
                        festivo = false;

                        if (duracion != "")
                        {
                            if (intDiaProc != 7 && !diaProcIsFiesta)
                            {
                                anadirDia = true;
                            }
                            else
                            {
                                if (intDiaProc == 7)
                                {
                                    anadirDia = festivo = true;
                                }
                            }
                        }
                        else
                        {
                            if (diaProcIsFiesta && dtServ.Rows[0]["Domingo"] != null)
                            {
                                anadirDia = festivo = true;
                                bool prestaServicioEnFestivo = await UsuarioNoPrestaServEnFestivo(expUsuario);
                                if (Weekday(diaProc) != 7 && prestaServicioEnFestivo)
                                { 
                                    anadirDia = false;
                                }

                            }
                        }

                        if (anadirDia)
                        {
                            myId = Guid.NewGuid().ToString();

                            sql = $@"
                                EXECUTE SP_Crear_HorarioAuxiliares_TP
                                @ExpAuxiliar_1 = {expAuxiliar}
                                ,@DiaHorario_2 = '{diaProc}'
                                ,@NumeroSemana_3 = {NumeroSemana(diaProc)}
                                ,@HoraInicio_4 = '{horaInicio}'
                                ,@Duracion_5 = '{duracion}'
                                ,@ExpUsuario_6 = {expUsuario}
                                ,@CodigoServicio_7 = {codServicio}
                                ,@TitularSuplente_8 = 'T'
                                ,@RefCodigoHorario_9 = 0
                                ,@AuxUsu_10 = ''
                                ,@Notas_11 = ''
                                ,@FestivaSi_12 = {Math.Abs(Convert.ToInt16(festivo))}
                                ,@Regimen_13 = {regimen}
                                ,@ConceptoHoras_14 = 0
                                ,@UsuarioAvisado_15 = 0
                                ,@myID_16 = '{myId}'
                                ,@Fecha_17 = '{DateTime.Now}'";

                            if (proyeccion == 0)
                            {
                                sql += $@"
                                ,@TieneAseoEventual_18 = NULL
                                ,@TPublico_19 = {tPublicoDia}
                                ,@InicioTarde_20 = {inicioTarde}
                                ,@NoFichaEntrada_21 = {noFichaEntradaDia}
                                ,@NoFichaSalida_22 = {noFichaSalidaDia}";

                                if (AC_ExpUsuario2 > 0)
                                {
                                    sql += $@"
                                ,@AC_ExpUsuario2 = {AC_ExpUsuario2}";
                                }

                                var resultado = await _dataService.EjecutarComandoAsync(sql);

                                /// TODO: Mirar a ver si puedo recoger el nº de errores
                            }
                        }
                    }
                }
            }
            return 0;
        }

        private static int Weekday(DateTime fecha)
        {
            if (fecha.DayOfWeek == DayOfWeek.Monday) return 1;
            if (fecha.DayOfWeek == DayOfWeek.Tuesday) return 2;
            if (fecha.DayOfWeek == DayOfWeek.Wednesday) return 3;
            if (fecha.DayOfWeek == DayOfWeek.Thursday) return 4;
            if (fecha.DayOfWeek == DayOfWeek.Friday) return 5;
            if (fecha.DayOfWeek == DayOfWeek.Saturday) return 6;
            if (fecha.DayOfWeek == DayOfWeek.Sunday) return 7;
            return 0;
        }

        private static bool EsFestivo(int codDiaFiesta, DateTime diaFiesta, List<string> diasFestivos)
        {
            bool encontrado = false, festivo = false;

            foreach (string dia in diasFestivos)
            {
                if (!encontrado)
                {
                    string[] datosFestivo = dia.Split(CTE_SEPARADOR);
                    // Código del festivo en el primer campo, día en el segundo
                    if (Convert.ToInt16(datosFestivo[0]) == codDiaFiesta)
                    {
                        festivo = (Convert.ToDateTime(datosFestivo[1]) == diaFiesta);
                        encontrado = festivo;
                    }
                }
            }
            return festivo;
        }

        private static void CargarVariablesDiaRegHorario(DataTable dtServ, int intDiaProc, bool diaProcIsFiesta,
                                                    string strNoFichaEntrada, string strNoFichaSalida, 
                                                    out int noFichaEntradaDia, out int noFichaSalidaDia, out int inicioTarde,
                                                    out int tPublicoDia, out string horaInicio, out string duracion)
        {
            string strNoFichaDia;

            inicioTarde = tPublicoDia = 0;
            horaInicio = duracion = "";

            switch (intDiaProc)
            {
                case 1:
                    inicioTarde = Convert.ToInt16(ValNulo(dtServ.Rows[0]["InicioTarde_L"], "integer"));
                    tPublicoDia = Convert.ToInt16(ValNulo(dtServ.Rows[0]["TP_L"], "integer"));
                    horaInicio = ValNulo(dtServ.Rows[0]["InicioLunes"]).ToString();
                    duracion = ValNulo(dtServ.Rows[0]["Lunes"]).ToString();
                    break;

                case 2:
                    inicioTarde = Convert.ToInt16(ValNulo(dtServ.Rows[0]["InicioTarde_M"], "integer"));
                    tPublicoDia = Convert.ToInt16(ValNulo(dtServ.Rows[0]["TP_M"], "integer"));
                    horaInicio = ValNulo(dtServ.Rows[0]["InicioMartes"]).ToString();
                    duracion = ValNulo(dtServ.Rows[0]["Martes"]).ToString();
                    break;

                case 3:
                    inicioTarde = Convert.ToInt16(ValNulo(dtServ.Rows[0]["InicioTarde_X"], "integer"));
                    tPublicoDia = Convert.ToInt16(ValNulo(dtServ.Rows[0]["TP_X"], "integer"));
                    horaInicio = ValNulo(dtServ.Rows[0]["InicioMiercoles"]).ToString();
                    duracion = ValNulo(dtServ.Rows[0]["Miercoles"]).ToString();
                    break;

                case 4:
                    inicioTarde = Convert.ToInt16(ValNulo(dtServ.Rows[0]["InicioTarde_J"], "integer"));
                    tPublicoDia = Convert.ToInt16(ValNulo(dtServ.Rows[0]["TP_J"], "integer"));
                    horaInicio = ValNulo(dtServ.Rows[0]["InicioJueves"]).ToString();
                    duracion = ValNulo(dtServ.Rows[0]["Jueves"]).ToString();
                    break;

                case 5:
                    inicioTarde = Convert.ToInt16(ValNulo(dtServ.Rows[0]["InicioTarde_V"], "integer"));
                    tPublicoDia = Convert.ToInt16(ValNulo(dtServ.Rows[0]["TP_V"], "integer"));
                    horaInicio = ValNulo(dtServ.Rows[0]["InicioViernes"]).ToString();
                    duracion = ValNulo(dtServ.Rows[0]["Viernes"]).ToString();
                    break;

                case 6:
                    inicioTarde = Convert.ToInt16(ValNulo(dtServ.Rows[0]["InicioTarde_S"], "integer"));
                    tPublicoDia = Convert.ToInt16(ValNulo(dtServ.Rows[0]["TP_S"], "integer"));
                    horaInicio = ValNulo(dtServ.Rows[0]["InicioSabado"]).ToString();
                    duracion = ValNulo(dtServ.Rows[0]["Sabado"]).ToString();
                    break;

                case 7:
                    inicioTarde = Convert.ToInt16(ValNulo(dtServ.Rows[0]["InicioTarde_D"], "integer"));
                    tPublicoDia = Convert.ToInt16(ValNulo(dtServ.Rows[0]["TP_D"], "integer"));
                    horaInicio = ValNulo(dtServ.Rows[0]["InicioDomingo"]).ToString();
                    duracion = ValNulo(dtServ.Rows[0]["Domingo"]).ToString();
                    break;
            }

            strNoFichaDia = strNoFichaEntrada.Substring(intDiaProc - 1, 1);

            noFichaEntradaDia = 0;
            if (strNoFichaEntrada.Length == 7 && IsNumeric(strNoFichaDia))
            {
                noFichaEntradaDia = Convert.ToInt16(strNoFichaDia);
            }

            noFichaSalidaDia = 0;
            if (strNoFichaSalida.Length == 7 && IsNumeric(strNoFichaDia))
            {
                noFichaSalidaDia = Convert.ToInt16(strNoFichaDia);
            }

            if (duracion != null && duracion.Equals(string.Empty) && diaProcIsFiesta && dtServ.Rows[0]["Domingo"] != null)
            {
                inicioTarde = Convert.ToInt16(ValNulo(dtServ.Rows[0]["InicioTarde_D"], "integer"));
                tPublicoDia = Convert.ToInt16(ValNulo(dtServ.Rows[0]["TP_D"], "integer"));
                horaInicio = ValNulo(dtServ.Rows[0]["InicioDomingo"]).ToString();
                duracion = ValNulo(dtServ.Rows[0]["Domingo"]).ToString();

                strNoFichaDia = strNoFichaEntrada.Substring(intDiaProc - 1, 1);

                noFichaEntradaDia = 0;
                if (strNoFichaEntrada.Length == 7 && IsNumeric(strNoFichaDia))
                {
                    noFichaEntradaDia = Convert.ToInt16(strNoFichaDia);
                }

                noFichaSalidaDia = 0;
                if (strNoFichaSalida.Length == 7 && IsNumeric(strNoFichaDia))
                {
                    noFichaSalidaDia = Convert.ToInt16(strNoFichaDia);
                }
            }
        }

        private async Task<bool> UsuarioNoPrestaServEnFestivo(Int64 expUsuario)
        {
            bool retVal = false;

            string sql = $@"
                SELECT expUsuario 
                FROM Usuarios_NoServFestivo
                WHERE expUsuario = {expUsuario}
                    AND NoPrestarServDiaFestivo = 1";

            DataTable resultado = await _dataService.EjecutarConsultaAsync(sql);

            if (resultado.Rows.Count > 0)
                retVal = true;

            return retVal;
        }

        private static int NumeroSemana(DateTime diaProc)
        {
            Calendar calendar = CultureInfo.InvariantCulture.Calendar;

            return calendar.GetWeekOfYear(
                diaProc,
                CalendarWeekRule.FirstDay,   // Equivalente a vbFirstJan1
                DayOfWeek.Monday             // Equivalente a vbMonday
            );
        }

        public static object ValNulo(object valor, string tipoDatos = null, bool mayusculas = false)
        {
            try
            {
                // 1. Determinar tipo
                string tipo = !string.IsNullOrEmpty(tipoDatos)
                    ? tipoDatos
                    : (valor?.GetType().Name ?? "Null");

                // 2. Si es null o DBNull
                if (valor == null || valor == DBNull.Value)
                {
                    switch (tipo)
                    {
                        case "String":
                        case "Date":
                        case "Field":
                            return string.Empty;

                        case "Byte":
                        case "Integer":
                        case "Single":
                        case "Float":
                        case "Long":
                        case "Currency":
                        case "Decimal":
                        case "Boolean":
                            return 0;

                        default:
                            return null; // fallback razonable
                    }
                }

                // 3. Si es numérico
                if (valor is byte || valor is short || valor is int || valor is long ||
                    valor is float || valor is double || valor is decimal)
                {
                    return valor;
                }

                // 4. Si no es numérico, trabajamos como texto
                string texto = valor.ToString();

                return mayusculas ? texto.ToUpper() : texto;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error en ValNulo: {ex.Message}", ex);
            }
        }

        private static bool IsNumeric(object value)
        {
            if (value == null || value == DBNull.Value)
                return false;

            if (value is sbyte || value is byte ||
                value is short || value is ushort ||
                value is int || value is uint ||
                value is long || value is ulong ||
                value is float || value is double ||
                value is decimal)
                return true;

            // Como VB6: probar si se puede interpretar como número
            return double.TryParse(
                value.ToString(),
                System.Globalization.NumberStyles.Any,
                System.Globalization.CultureInfo.InvariantCulture,
                out _
            );
        }
    }
}
