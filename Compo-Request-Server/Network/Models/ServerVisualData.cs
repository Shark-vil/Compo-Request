using Compo_Shared_Data.Network.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request_Server.Network.Models
{
    public delegate void ServerVisualDataDelegate(Receiver ReceiverData, UserNetwork UserNetwork = null);

    public class ServerVisualData
    {
        public int WindowUid;
        public string UserUid;
        public string KeyNetwork;
        public ServerVisualDataDelegate DataDelegate;

        public ServerVisualData(ServerVisualDataDelegate DataDelegate, string KeyNetwork = null, int WindowUid = -1, string UserUid = null)
        {
            this.WindowUid = WindowUid;
            this.UserUid = UserUid;
            this.KeyNetwork = KeyNetwork;
            this.DataDelegate = DataDelegate;
        }
    }
}
