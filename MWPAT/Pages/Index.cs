using Azure.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.JSInterop;
using SiteManage.Services;
using SiteManage.Services.Handler;
using SiteManage.Services.SocketUitils;
using SiteManage.Services.SocketUitils.Interfaces;


namespace SiteManage.Pages
{
    public partial class Index
    {

        [Inject] public ISocketService socketService { get; set; }
        [Inject] public IConfiguration configuration { get; set; }
        //private readonly ILogger<HandlerClient> _logger;
        [Inject] public ILogger<Index> _logger { get; set; }
        [Inject] public IJSRuntime jsRuntime { get; set; }
        public string? Puerto { get; set; }
        public bool stateButton { get; set; }
        public bool stateButtonTask { get; set; }
        public string messageError { get; set; }
        public string messageSuccess { get; set; }
        public DateTime DateLog{ get; set; }

        protected override void OnInitialized()
        {
            stateButtonTask = true;
            Puerto = "";
            messageError = "";
            DateLog =  DateTime.Now;
         

            // stateButton = true  ->  Boton Start
            // statebUTTON = false ->  Boton Stop

            if (socketService.EstadoSocket()) // VERIFICA SI ENCUENTRA EL SERVIDOR SOCKET INICIADO
            {
                stateButton = false; // SI LO ENCUENTRA INICIADO EL BOTON START SE OCULTA Y APARECE EL BOTON STOP
                Puerto = socketService.PuertoEnUso();
            }
            else
            {
                Puerto = configuration["ENV:PORT"] ?? "";
                stateButton = true;  // SI NO LO ENCUENTRA INICIADO EL BOTON STOP SE OCULTA Y APARECE EL BOTON START 
            }

            string carpeta = configuration["ENV:LOGPATH"];

            if (Directory.Exists(carpeta)) {
                string[] archivos = Directory.GetFiles(carpeta);
                Console.WriteLine("Archivos en Carpeta");
                foreach (string archivo in archivos) { 
                    Console.WriteLine(Path.GetFileName(archivo));
                }
            } else {
                Console.WriteLine("La carpeta no existe");
            }
        }

        public void StartSocketService()
        {
            messageError = "";

            // VERIFICA QUE EL PUERTO NO VENGA VACIO
            if (Puerto.Equals(""))
            {
                messageError = "El puerto es necesario";
                StateHasChanged();
                _logger.LogInformation($"[SERVER] - El servidor no se pudo iniciar porque no se especifico el puerto");
                return;
            }

            int puerto = Convert.ToInt32(Puerto);
            

            // VERIFICAR QUE EL PUERTO SEA MAYO A 1023 Y MENOR A 49152
            if (puerto < 1023 || puerto > 49152) {
                messageError = "El puerto debe estar entre 1024 y 49151";
                StateHasChanged();
                _logger.LogInformation($"[SERVER] - El servidor no se pudo iniciar porque puerto {Puerto} no entra en el rango permitido");
                return;
            }

            //VERIFICAR SI EL PUERTO ESTA EN USO
            if (socketService.VerificarPuerto(puerto))
            {
                messageError = $"El puerto {Puerto} esta en uso";
                StateHasChanged();
                _logger.LogInformation($"[SERVER] - El servidor no se pudo iniciar porque el puerto {Puerto} se encuentra en uso por otra aplicacion");
                return;
            }


            socketService.Run(puerto);
            stateButton = false;
            

        }
        

        private void StopSocketService()
        {
            socketService.Stop();
            stateButton = true;
            _logger.LogInformation($"[SERVER] - Se detuvo el servidor para aceptar tramas en el puerto: {Puerto}");

        }

        private async Task SendTask() 
        {
            messageError = "";
            messageSuccess = "";
            stateButtonTask = false;
            StateHasChanged();
            _logger.LogInformation($"[CLIENT] Ejecutando tarea {DateTime.Now}");
            ///ResponseModel respuesta = await socketService.ExecuteTask();
            stateButtonTask = true;
            /*
            if (respuesta.Response)
                messageSuccess = respuesta.MessageResponse;
            else
                messageError = respuesta.MessageResponse;
            */
        }

        private async Task DownloadLog() {
           
            Guid guid = Guid.NewGuid();
            string url = $"{configuration["ENV:LOGPATH"]}MWPATLOG-{DateLog.ToString("yyyyMMdd")}.txt";
            string copy = $"{configuration["ENV:LOGPATH"]}{guid}.txt";

            try {
                File.Copy(url, copy);
                byte[] contenido = File.ReadAllBytes(copy);         
                await jsRuntime.InvokeAsync<object>("downloadFile", $"MWPATLOG-({DateLog.ToString("dd-MM-yyyy")}).txt", contenido);
                File.Delete(copy);
            }
            catch (Exception e) {
                await jsRuntime.InvokeAsync<object>("alerta", "No se encontro ningun LOG para era fecha");
            }
            
        }
      
    }
}
