using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Compo_Request_Server.Network.Models
{
    [Serializable]
    public class User
    {
        public int Id;
        public string Ip;
        public int Port;
        public IPEndPoint NetPoint;
    }
}
