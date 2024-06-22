using Microsoft.IdentityModel.Tokens;
using SiteManage.Services.EF;
using SiteManage.Services.Handler;
using SiteManage.Services.Handler.Interfaces;
using SiteManage.Services.SocketUitils.Interfaces;
using System.Net;

namespace SiteManage.Services.SocketUitils
{
    public class SocketService : ISocketService
    {
        private readonly IConnectionSocket _connectionSocket;
        private readonly IHandlerClient _handlerClient;
        private readonly IHandlerServer _handlerServer;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SocketService> _logger;
        private string TERMINAL;
        private string IP;
        private string PORT;
        private string TIEMPO_ESPERA;

        public SocketService(IConnectionSocket connectionSocket, IHandlerServer handlerServer,IHandlerClient handlerClient,IConfiguration configuration, ILogger<SocketService> logger)
        {
            _connectionSocket = connectionSocket;
            _handlerClient = handlerClient;
            _handlerServer = handlerServer;
            _configuration = configuration;
            _logger = logger;
            IP = _configuration["ENV:IP_COREQ"]??"";
            PORT = _configuration["ENV:PORT_COREQ"]?? "";
            TERMINAL = _configuration["ENV:TERMINAL"] ?? "";
            TIEMPO_ESPERA = _configuration["ENV:TIEMPO_ESPERA"] ?? "15000";
        }
       
        // ESTE METODO EJECUTA LA TAREA PROGRAMADA 
        public async Task<ResponseModel> ExecuteTask()
        {
            await Task.Delay(10);
            return _handlerClient.Handler(IP, Convert.ToInt16(PORT),TERMINAL,Convert.ToInt32(TIEMPO_ESPERA));
        }

        // ESTE METODO ME DEVUELVE SI EL SERVIDOR ESTA CORRIENDO O NO
        public bool EstadoSocket()
        {
            return _connectionSocket.VerificarEstadoSocket();
        }

        public string PuertoEnUso() {
            return _connectionSocket.ObtenerPuerto();
        }

        public bool VerificarPuerto(int Puerto) {
            return _connectionSocket.VerificarPuerto(Puerto);
        }

        // ESTE METODO INICIA EL SERVIDOR SOCKET
        public async Task Run(int Port)
        {
           
            while (true) {
                try
                {
                    var listener = _connectionSocket.OpenConnection(Port);
                    _logger.LogInformation($"[SERVER] - Se inicio el servidor para aceptar tramas en el puerto: {Port}");
                    while (true)
                    {
                        var client = await listener.AcceptTcpClientAsync();
                        _ = _handlerServer.Handler(client, TERMINAL);
                    }

                }
                catch
                {
                    _logger.LogInformation($"[SERVER] - El Servidor se detuvo inesperadamente");
                }
                finally {
                    _connectionSocket.CloseConnection();
                }
            
            }
        }

        // ESTE METODO DETIENE EL SERVIDOR SOCKET
        public void Stop()
        {
            _connectionSocket.CloseConnection();
        }
    }
}
