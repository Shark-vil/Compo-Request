using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Compo_Request_Server.Network.Models
{
    [Serializable]
    public class MNetworkClient
    {
        public string Id { get; private set; }
        public string Ip { get; private set; }
        public int Port { get; private set; }
        public Socket ClientNetwork { get; private set; }
        public IPEndPoint NetPoint { get; private set; }

        public MNetworkClient(Socket ClientNetwork)
        {
            Id = Guid.NewGuid().ToString();
            NetPoint = (IPEndPoint)ClientNetwork.RemoteEndPoint;
            Ip = NetPoint.Address.ToString();
            Port = NetPoint.Port;
            this.ClientNetwork = ClientNetwork;
        }
    }
}
