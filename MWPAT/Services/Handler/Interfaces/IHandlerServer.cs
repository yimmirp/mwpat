using System.Net.Sockets;

namespace SiteManage.Services.Handler.Interfaces
{
    public interface IHandlerServer
    {
        public Task Handler(TcpClient client, string Terminal);
    }
}
