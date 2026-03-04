namespace CargaDatos.Web.Services
{
    public class CargaDatosProgramadaService : BackgroundService
    {
        private readonly ILogger<CargaDatosProgramadaService> _logger;
        private readonly TimeSpan _intervalo = TimeSpan.FromHours(7);

        public CargaDatosProgramadaService(ILogger<CargaDatosProgramadaService> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Servicio de carga programada iniciado. Se ejecutará cada 7 horas.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await EjecutarCargaProgramada();
                    
                    _logger.LogInformation($"Próxima ejecución en 7 horas a las: {DateTime.Now.Add(_intervalo):dd/MM/yyyy HH:mm:ss}");
                    await Task.Delay(_intervalo, stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    _logger.LogInformation("Servicio de carga programada detenido");
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error en el servicio de carga programada");
                    // Esperar un tiempo antes de reintentar en caso de error
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

        private async Task EjecutarCargaProgramada()
        {
            _logger.LogInformation($"Ejecutando carga programada a las: {DateTime.Now:dd/MM/yyyy HH:mm:ss}");
            
            // TODO: Aquí puedes implementar la lógica que necesites
            // Por ejemplo, podrías llamar a un servicio, ejecutar una carga automática, etc.
            
            await Task.CompletedTask;
            
            _logger.LogInformation("Carga programada completada");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deteniendo servicio de carga programada...");
            await base.StopAsync(cancellationToken);
        }
    }
}
