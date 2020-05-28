using Compo_Shared_Data.Debugging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Compo_Request_Server.Network
{
    public class NetworkBase
    {
        public static int Port;
        public static IPEndPoint NetPoint;
        public static Socket Listener;

        public static List<Models.MNetworkClient> NetworkClients = new List<Models.MNetworkClient>();

        public static void Setup(string ServerIp, int ServerPort)
        {
            Port = ServerPort;
            NetPoint = new IPEndPoint(IPAddress.Parse(ServerIp), ServerPort);
            Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Listener.Bind(NetPoint);

            Debug.Log($"Address - {NetPoint.Address}:{ServerPort}\r\n");
        }
    }
}
