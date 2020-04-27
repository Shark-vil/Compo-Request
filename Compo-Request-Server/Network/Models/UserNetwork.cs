using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Compo_Request_Server.Network.Models
{
    [Serializable]
    public class UserNetwork
    {
        public string Id { get; private set; }
        public string Ip { get; private set; }
        public int Port { get; private set; }
        public NetworkStream Stream { get; private set; }
        public TcpClient ClientNetwork { get; private set; }
        public IPEndPoint NetPoint { get; private set; }

        public UserNetwork(TcpClient ClientNetwork)
        {
            Id = Guid.NewGuid().ToString();
            NetPoint = (IPEndPoint)ClientNetwork.Client.RemoteEndPoint;
            Ip = NetPoint.Address.ToString();
            Port = NetPoint.Port;
            Stream = ClientNetwork.GetStream();
            this.ClientNetwork = ClientNetwork;
        }
    }
}
