using Microsoft.IdentityModel.Tokens;
using ServiceSocketXW.Core.Socket.Interfaces;

namespace ServiceSocketXW
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly ISocketService _socketService;
        private readonly IConfiguration _configuration;
        private string? Puerto { get; set; }

        public Worker(ILogger<Worker> logger, ISocketService socketService, IConfiguration configuration)
        {
            _socketService = socketService;
            _logger = logger;
            _configuration = configuration;
            Puerto = _configuration["ENV:PORT"] ?? "";
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {

            // VERIFICA QUE EL PUERTO NO VENGA VACIO
            if (Puerto.IsNullOrEmpty())
                _logger.LogInformation($"[SERVER] - El servidor no se pudo iniciar porque no se especifico el puerto");

            // VERIFICA QUE EL PUERTO NO VENGA VACIO
            if (Puerto.Equals(""))
                _logger.LogInformation($"[SERVER] - El servidor no se pudo iniciar porque no se especifico el puerto");


            int puerto = Convert.ToInt32(Puerto);


            // VERIFICAR QUE EL PUERTO SEA MAYO A 1023 Y MENOR A 49152
            if (puerto < 1023 || puerto > 49152)          
                _logger.LogInformation($"[SERVER] - El servidor no se pudo iniciar porque puerto {Puerto} no entra en el rango permitido");


            if (_socketService.VerificarPuerto(puerto))
                _logger.LogInformation($"[SERVER] - El servidor no se pudo iniciar porque el puerto {Puerto} se encuentra en uso por otra aplicacion");

            _socketService.Run(puerto);

        }
    }
}