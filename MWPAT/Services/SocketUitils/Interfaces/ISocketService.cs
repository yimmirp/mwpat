namespace SiteManage.Services.SocketUitils.Interfaces
{
    public interface ISocketService
    {
        public Task Run(int Port);
        public void Stop();
        public bool EstadoSocket();
        //public Task<ResponseModel> ExecuteTask();
        public string PuertoEnUso();
        public bool VerificarPuerto(int Puerto);
    }
}
