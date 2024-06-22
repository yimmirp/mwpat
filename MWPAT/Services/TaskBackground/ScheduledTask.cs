using SiteManage.Services.Handler;
using SiteManage.Services.Handler.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace SiteManage.Services.TaskBackground
{
    public class ScheduledTask : IHostedService, IDisposable
    {
        private Timer _timer;
        private DateTime _nextExecutionTime;
        private readonly ILogger<ScheduledTask> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IConfiguration _configuration;


        public ScheduledTask(ILogger<ScheduledTask> logger, IServiceScopeFactory serviceScopeFactory, IConfiguration configuration)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _configuration = configuration;
            var HORA = _configuration["ENV:HORA"];
            var MINUTOS = _configuration["ENV:MINUTOS"];
            _nextExecutionTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, Convert.ToInt16(HORA), Convert.ToInt16(MINUTOS), 59, 999);
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {

            if(DateTime.Now > _nextExecutionTime)
                _nextExecutionTime = _nextExecutionTime.AddDays(1);
      

            TimeSpan initialDelay = _nextExecutionTime - DateTime.Now;
            _logger.LogInformation($"[CLIENT] Tarea programada para {_nextExecutionTime}.");
            _timer = new Timer(Execute, null, initialDelay, TimeSpan.FromDays(1));
           
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Execute(object state) {
           
            _logger.LogInformation($"[CLIENT] Ejecutando tarea {DateTime.Now}");
            _nextExecutionTime = _nextExecutionTime.AddMinutes(1);
            _timer.Change(TimeSpan.FromMinutes(1), TimeSpan.Zero);

            var IP = _configuration["ENV:IP_COREQ"];
            var PORT = _configuration["ENV:PORT_COREQ"];
            var TERMINAL = _configuration["ENV:TERMINAL"];
            var TIEMPO_ESPERA = _configuration["ENV:TIEMPO_ESPERA"];
            using (var scope = _serviceScopeFactory.CreateScope()) {
                var handleClient = scope.ServiceProvider.GetService<IHandlerClient>();
                _ = handleClient.Handler(IP, Convert.ToInt16(PORT), TERMINAL,Convert.ToInt32(TIEMPO_ESPERA));
            }

                _logger.LogInformation($"[CLIENT] Proxima tarea programada a las {_nextExecutionTime}");

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
