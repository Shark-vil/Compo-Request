using Compo_Shared_Data.Debugging;
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

        public static bool Setup(string ServerHost, int ServerPort)
        {
            if (ClientNetwork != null && ClientNetwork.Connected)
                return true;
            else
                Debug.LogWarning("Не удаётся установить соединение с сервером");

            Host = ServerHost;
            Port = ServerPort;

            /*
            NetHost = Dns.GetHostEntry(Host);
            NetAddress = NetHost.AddressList[0];

            NetPoint = new IPEndPoint(NetAddress, ServerPort);
            */

            NetPoint = new IPEndPoint(IPAddress.Parse(ServerHost), ServerPort);

            try
            {
                ClientNetwork = new Socket(NetPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                ClientNetwork.Connect(NetPoint);

                return true;
            }
            catch(Exception ex)
            {
                Debug.LogError("Connection Setup:\n" + ex);
                return false;
            }
        }
    }
}
