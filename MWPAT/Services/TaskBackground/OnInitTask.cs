using Microsoft.IdentityModel.Tokens;
using SiteManage.Services.Handler;
using SiteManage.Services.SocketUitils;
using SiteManage.Services.SocketUitils.Interfaces;

namespace SiteManage.Services.TaskBackground
{
    public class OnInitTask : IHostedService
    {
        private readonly ISocketService _socketService;
        private readonly ILogger<OnInitTask> _logger;
        private readonly IConfiguration _configuration;
        private string? Puerto { get; set; }
        public OnInitTask(ISocketService socketService, ILogger<OnInitTask> logger, IConfiguration configuration)
        {
            _socketService = socketService;
            _logger = logger;
            _configuration = configuration;
            Puerto = _configuration["ENV:PORT"] ?? "";
        }


        public Task StartAsync(CancellationToken cancellationToken)
        {
            // VERIFICA QUE EL PUERTO NO VENGA VACIO
            if (Puerto.IsNullOrEmpty())
            {

                _logger.LogInformation($"[SERVER] - El servidor no se pudo iniciar porque no se especifico el puerto");
                return Task.CompletedTask;
            }

            // VERIFICA QUE EL PUERTO NO VENGA VACIO
            if (Puerto.Equals(""))
            {

                _logger.LogInformation($"[SERVER] - El servidor no se pudo iniciar porque no se especifico el puerto");
                return Task.CompletedTask;
            }

            int puerto = Convert.ToInt32(Puerto);


            // VERIFICAR QUE EL PUERTO SEA MAYO A 1023 Y MENOR A 49152
            if (puerto < 1023 || puerto > 49152)
            {

                _logger.LogInformation($"[SERVER] - El servidor no se pudo iniciar porque puerto {Puerto} no entra en el rango permitido");
                return Task.CompletedTask;
            }

            if (_socketService.VerificarPuerto(puerto))
            {

                _logger.LogInformation($"[SERVER] - El servidor no se pudo iniciar porque el puerto {Puerto} se encuentra en uso por otra aplicacion");
                return Task.CompletedTask;
            }


            _socketService.Run(puerto);




            return Task.CompletedTask;

        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;

        }
    }
}
