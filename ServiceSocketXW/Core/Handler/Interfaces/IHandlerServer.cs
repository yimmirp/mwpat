using System.Net.Sockets;

namespace ServiceSocketXW.Core.Handler.Interfaces
{
    public interface IHandlerServer
    {
        public Task Handler(TcpClient client, string Terminal);
    }
}
