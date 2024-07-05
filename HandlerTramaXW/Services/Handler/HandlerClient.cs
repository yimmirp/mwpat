using HandlerTramaXW.Services.Handler.DTO;
using HandlerTramaXW.Services.Handler.Interfaces;
using System.Net;
using System.Net.Sockets;

namespace HandlerTramaXW.Services.Handler
{
    public class HandlerClient : IHandlerClient
    {
        //private readonly IPagoService _pagoService;
        private readonly ILogger<HandlerClient> _logger;
        public HandlerClient(ILogger<HandlerClient> logger)
        {
            _logger = logger;
        }

        public ResponseModel Handler(string IPCoreQ, int PortCoreQ, string Terminal, int TIEMPO_ESPERA)
        {
            ResponseModel mensajeRespuesta = new ResponseModel()
            {
                Response = true,
                MessageResponse = $"Tarea ejecutada con exito"
            };
            var IDTransaction = Guid.NewGuid();
            XwalkEncryption EncodeXW = new XwalkEncryption();
            IPAddress IP = IPAddress.Parse(IPCoreQ);
            IPEndPoint ipEndPoint = new IPEndPoint(IP, PortCoreQ);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _logger.LogInformation($"[CLIENT][TransactionID: {IDTransaction}] - Se abrio conexion con cliente {IPCoreQ}:{PortCoreQ}");
            try
            {
                socket.Connect(ipEndPoint); // Conexión sincrónica
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"[CLIENT][TransactionID: {IDTransaction}] - Error en la conexion con el servidor {IPCoreQ}:{PortCoreQ}");

            }


            try
            {
                // Obtener Fecha Actual
                string todayDate_T = DateTime.Now.ToString("ddMMMyy", System.Globalization.CultureInfo.InvariantCulture).ToUpper();
                string date_T = DateTime.Now.ToString("yyMM").ToUpper();
                string today = DateTime.Now.AddDays(-1).ToString("ddMMyyyy").ToUpper();

                string TramaSended = $"000090000000{Terminal.PadRight(10, ' ').Substring(0, 10)}00100PTXMWT000000{todayDate_T}{date_T}{today}1";
                _logger.LogInformation($"[CLIENT][TransactionID: {IDTransaction}] - Trama Enviada: {TramaSended}");
                byte[] TramaEBCDIC = EncodeXW.ASCIItoEBCDIC(TramaSended);
                socket.Send(TramaEBCDIC);

                byte[] response = new byte[100];

                // Configurar un temporizador para esperar 15 segundos para la respuesta del servidor
                var timer = new Timer((state) =>
                {
                    _logger.LogInformation($"[CLIENT][TransactionID: {IDTransaction}] - Tiempo de espera agotado. No se pudo recibir la respuesta del servidor");
                    socket.Close(); // Cerrar la conexión si el tiempo de espera se agota

                }, null, TIEMPO_ESPERA, Timeout.Infinite);

                try
                {
                    int bytesRead = socket.Receive(response);
                    timer.Change(Timeout.Infinite, Timeout.Infinite); // Detener el temporizador

                    if (bytesRead > 0)
                    {
                        var buffer = new List<byte>(response);
                        string TramaReceived = EncodeXW.EBCDICtoASCII(buffer);
                        _logger.LogInformation($"[CLIENT][TransactionID: {IDTransaction}] - Trama Recibida: {TramaReceived}");
                        string Respuesta = TramaReceived.Substring(58, 1);
                        if (Respuesta == "0")
                        {
                            mensajeRespuesta.Response = false;
                            mensajeRespuesta.MessageResponse = TramaReceived.Substring(59, 41);
                        }

                    }
                    else
                    {
                        _logger.LogInformation($"[CLIENT][TransactionID: {IDTransaction}] - No se recibió respuesta del servidor");

                    }
                }
                catch (SocketException ex)
                {
                    _logger.LogInformation($"[CLIENT][TransactionID: {IDTransaction}] - Error al recibir la respuesta del servidor");
                    mensajeRespuesta.Response = false;
                    mensajeRespuesta.MessageResponse = $"El host: {IPCoreQ}:{PortCoreQ} no devolvio respuesta";

                }

                socket.Close();
                _logger.LogInformation($"[CLIENT][TransactionID: {IDTransaction}] - Se cerro la conexion con cliente: {IPCoreQ}:{PortCoreQ}");
                return mensajeRespuesta;

            }
            catch (Exception ex)
            {
                _logger.LogInformation($"[CLIENT][TransactionID: {IDTransaction}] - {ex}");
                return new ResponseModel()
                {
                    Response = false,
                    MessageResponse = $"Ocurrio un problema en la aplicacion"
                };
            }
        }
    }
}
