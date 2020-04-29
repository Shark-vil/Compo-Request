using Compo_Shared_Data.Network.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request_Server.Network.Models
{
    public delegate void NetworkDelegateTemplate(MResponse ReceiverData, MNetworkClient NetworkClient = null);

    public class MNetworkAction
    {
        public int WindowUid;
        public string KeyNetwork;
        public NetworkDelegateTemplate DataDelegate;

        public MNetworkAction(NetworkDelegateTemplate DataDelegate, string KeyNetwork = null, int WindowUid = -1)
        {
            this.WindowUid = WindowUid;
            this.KeyNetwork = KeyNetwork;
            this.DataDelegate = DataDelegate;
        }
    }
}
