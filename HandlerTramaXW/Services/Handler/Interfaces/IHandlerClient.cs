using HandlerTramaXW.Services.Handler.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace HandlerTramaXW.Services.Handler.Interfaces
{
    public interface IHandlerClient
    {
        public ResponseModel Handler(string IPCoreQ, int PortCoreQ, string Terminal, int TIEMPO_ESPERA);
    }
}
