using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ServiceSocketXW.Core.Socket.Interfaces
{
    public interface IConnectionSocket
    {
        public TcpListener OpenConnection(int Port);
        public bool CloseConnection();
        public bool VerificarPuerto(int Puerto);
    }
}
