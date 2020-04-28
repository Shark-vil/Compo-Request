using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Compo_Request.Network
{
    public class NetworkBase
    {
        public static string Host;
        public static int Port;

        public static IPHostEntry NetHost;
        public static IPAddress NetAddress;

        public static IPEndPoint NetPoint;
        public static Socket ClientNetwork;

        public static void Setup(string ServerHost, int ServerPort)
        {
            Host = ServerHost;
            Port = ServerPort;

            NetHost = Dns.GetHostEntry(Host);
            NetAddress = NetHost.AddressList[0];

            NetPoint = new IPEndPoint(NetAddress, ServerPort);

            try
            {
                ClientNetwork = new Socket(NetAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                ClientNetwork.Connect(Host, Port);
            }
            catch(Exception ex)
            {

            }
        }
    }
}
