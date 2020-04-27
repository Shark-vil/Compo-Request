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
        public static TcpListener Listener;

        public static List<Models.UserNetwork> UsersNetwork = new List<Models.UserNetwork>();

        public static void Setup(int ServerPort)
        {
            Port = ServerPort;
            NetPoint = new IPEndPoint(IPAddress.Any, ServerPort);
            Listener = new TcpListener(NetPoint);
        }
    }
}
