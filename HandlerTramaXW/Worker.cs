using HandlerTramaXW.Services.Handler;
using HandlerTramaXW.Services.Handler.Interfaces;

namespace HandlerTramaXW
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHandlerClient _handlerClient;
        private readonly IConfiguration _configuration;
        private Timer _timer;
        private string TERMINAL;
        private string IP;
        private string PORT;
        private string TIEMPO_ESPERA;
        private string HORA;
        private string MINUTOS;


        public Worker(ILogger<Worker> logger, IConfiguration configuration,IHandlerClient handlerClient)
        {
            _logger = logger;
            _handlerClient = handlerClient;
            _configuration = configuration;
            IP = _configuration["ENV:IP_COREQ"] ?? "";
            PORT = _configuration["ENV:PORT_COREQ"] ?? "";
            TERMINAL = _configuration["ENV:TERMINAL"] ?? "";
            TIEMPO_ESPERA = _configuration["ENV:TIEMPO_ESPERA"] ?? "15000";
            HORA = _configuration["ENV:HORA"] ?? "04";
            MINUTOS = _configuration["ENV:MINUTOS"] ?? "59";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("HandlerXW iniciado : {time}", DateTimeOffset.Now);
            ScheduleTask(stoppingToken);

        }

        private void ScheduleTask(CancellationToken stoppingToken)
        {
            DateTime now = DateTime.Now;
            DateTime nextRun = DateTime.Today.AddHours(Convert.ToInt16(HORA)).AddMinutes(Convert.ToInt16(MINUTOS)); // 5:00 AM today

            if (now > nextRun)
            {
                nextRun = nextRun.AddDays(1); // If it's already past 5:00 AM today, schedule for 5:00 AM tomorrow
            }

            TimeSpan initialDelay = nextRun - now;
            _logger.LogInformation("La tarea se ejecutara a las {time}",nextRun);

            _timer = new Timer(ExecuteTask, null, initialDelay, Timeout.InfiniteTimeSpan);

            stoppingToken.Register(() => _timer?.Change(Timeout.Infinite, 0));
        }

        private void ExecuteTask(object state)
        {
            _logger.LogInformation("Ejecucion de tarea");

            // Call your scheduled method
            //HandlerClient _handlerClient = new HandlerClient();
            _handlerClient.Handler(IP, Convert.ToInt16(PORT), TERMINAL, Convert.ToInt32(TIEMPO_ESPERA));

            // Schedule the next run for 24 hours later
            _timer?.Change(TimeSpan.FromSeconds(10), Timeout.InfiniteTimeSpan);
            _logger.LogInformation("La siguiente tarea se ejecutara en 10 seg");
        }

        public override void Dispose()
        {
            _timer?.Dispose();
            base.Dispose();
        }
    }
}