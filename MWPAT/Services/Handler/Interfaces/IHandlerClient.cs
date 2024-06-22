namespace SiteManage.Services.Handler.Interfaces
{
    public interface IHandlerClient
    {
        public ResponseModel Handler(string IPCoreQ, int PortCoreQ, string Terminal, int TIEMPO_ESPERA);
    }
}
