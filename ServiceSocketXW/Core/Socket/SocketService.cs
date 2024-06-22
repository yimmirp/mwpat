using ServiceSocketXW.Core.Handler.Interfaces;
using ServiceSocketXW.Core.Socket.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace ServiceSocketXW.Core.Socket
{
    public class SocketService : ISocketService
    {
        private readonly IConnectionSocket _connectionSocket;
        private readonly IHandlerServer _handlerServer;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SocketService> _logger;
        private string TERMINAL;


        public SocketService(IConnectionSocket connectionSocket, IHandlerServer handlerServer, IConfiguration configuration, ILogger<SocketService> logger)
        {
            _connectionSocket = connectionSocket;
            _handlerServer = handlerServer;
            _configuration = configuration;
            _logger = logger;
            TERMINAL = _configuration["ENV:TERMINAL"] ?? "NOTERMINAL";
        }

        // ESTE METODO INICIA EL SERVIDOR SOCKET
        public async Task Run(int Port)
        {
            while (true)
            {
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
                finally
                {
                    _connectionSocket.CloseConnection();
                }
            }
        }

        //ESTE METODO DETIENE EL SERVIDOR SOCKET
        public void Stop()
        {
            _connectionSocket.CloseConnection();
        }
        //VERIFICA SI EL PUERTO A UTILIZARSE ESTA DISPONIBLE
        public bool VerificarPuerto(int Puerto)
        {

            // Obtiene todas las conexiones TCP activas
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnections = ipGlobalProperties.GetActiveTcpConnections();

            // Verifica si el puerto está siendo utilizado por alguna conexión activa
            foreach (var tcpConnection in tcpConnections)
            {
                if (tcpConnection.LocalEndPoint.Port == Puerto)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
