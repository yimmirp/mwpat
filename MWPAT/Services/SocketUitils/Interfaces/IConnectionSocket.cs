using System.Net.Sockets;

namespace SiteManage.Services.SocketUitils.Interfaces
{
    public interface IConnectionSocket
    {
        public TcpListener OpenConnection(int Port);
        public bool CloseConnection();
        public bool VerificarEstadoSocket();
        public string ObtenerPuerto();
        public bool VerificarPuerto(int Puerto);
    }
}
