using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceSocketXW.Core.Socket.Interfaces
{
    public interface ISocketService
    {
        public Task Run(int Port);
        public void Stop();
        public bool VerificarPuerto(int Puerto);
    }
}
