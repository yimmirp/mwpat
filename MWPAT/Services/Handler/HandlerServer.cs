using SiteManage.Data;
using SiteManage.Services.EF;
using SiteManage.Services.Handler.Interfaces;
using SiteManage.Services.SocketUitils;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SiteManage.Services.Handler
{
    public class HandlerServer : IHandlerServer
    {
        private readonly IServiceProvider _serviceProvider;

        //private readonly IPagoService _pagoService;
        private readonly ILogger<HandlerServer> _logger;

        public HandlerServer(IServiceProvider serviceProvider, ILogger<HandlerServer> logger)
        {
            _serviceProvider = serviceProvider;
            //_pagoService = pagoService;
            _logger = logger;
        }

        public async Task Handler(TcpClient client, string Terminal)
        {
            var IDTransaction = Guid.NewGuid();
            IPEndPoint? clientEndPoint = client.Client.RemoteEndPoint as IPEndPoint;
            string clientIPAddress = clientEndPoint.Address.ToString();
            int clientPort = clientEndPoint.Port;
            _logger.LogInformation($"[SERVER][TransactionID: {IDTransaction}] - Cliente {clientIPAddress}:{clientPort} conectado.");
            try
            {
                NetworkStream stream = client.GetStream();
                XwalkEncryption EncodeXW = new XwalkEncryption();
                var buffer = new List<byte>();
                int bytesRead = 0;

                do
                {
                    byte[] tempBuffer = new byte[1024];
                    bytesRead = await stream.ReadAsync(tempBuffer, 0, tempBuffer.Length);
                    if (bytesRead > 0)
                        buffer.AddRange(tempBuffer.Take(bytesRead));

                } while (buffer[buffer.Count - 1] != 239 && buffer[buffer.Count - 2] != 255);

                string TramaASCII = EncodeXW.EBCDICtoASCII(buffer);
                _logger.LogInformation($"[SERVER][TransactionID: {IDTransaction}] - Trama Recibida: {TramaASCII}.");
                string respuesta = "";
                using (var scope = _serviceProvider.CreateScope())
                {
                    var _pagoService = scope.ServiceProvider.GetRequiredService<IPagoService>();
                    respuesta = _pagoService.ConsultarTBL_PAGO(IDTransaction, TramaASCII, Terminal);
                }
                    
                _logger.LogInformation($"[SERVER][TransactionID: {IDTransaction}] - Trama Enviada: {respuesta}.");
                byte[] TramaEBCDIC = EncodeXW.ASCIItoEBCDIC(respuesta);
                await stream.WriteAsync(TramaEBCDIC, 0, TramaEBCDIC.Length);
            }
            catch (Exception ex)
            { 
                _logger.LogInformation($"[SERVER][TransactionID: {IDTransaction}] - Error handling client: {ex.Message}.");
            }
            client.Close();
            _logger.LogInformation($"[SERVER][TransactionID: {IDTransaction}] - Cliente {clientIPAddress}:{clientPort} desconectado.");
        }
    }
}
