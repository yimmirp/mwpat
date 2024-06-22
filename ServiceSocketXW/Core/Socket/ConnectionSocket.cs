﻿using ServiceSocketXW.Core.Socket.Interfaces;
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;

namespace ServiceSocketXW.Core.Socket
{
    public class ConnectionSocket: IConnectionSocket
    {
        private TcpListener _Listener;
        private bool isListenerRunning = false;
        private string _Port = "";

        public TcpListener OpenConnection(int Port)
        {
            _Listener = new TcpListener(IPAddress.Any, Port);
            _Listener.Start();
            _Port = Port.ToString();
            isListenerRunning = true;
            return _Listener;
        }
        public bool CloseConnection()
        {
            _Listener.Stop();
            _Port = "";
            isListenerRunning = false;
            return true;
        }
        public bool VerificarPuerto(int Puerto)
        {

            // Obtiene todas las conexiones TCP activas
            IPGlobalProperties ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] tcpConnections = ipGlobalProperties.GetActiveTcpConnections();

            // Verifica si el puerto está siendo utilizado por alguna conexión activa
            foreach (var tcpConnection in tcpConnections)
            {
                if (tcpConnection.LocalEndPoint.Port == Puerto)
                {
                    return true;
                }
            }

            return false;
        }

    }
}
