using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;
using Compo_Shared_Data.Network.Models;

namespace Compo_Request.Network.Models
{
    public delegate void NetworkDelegateTemplate(MResponse ReceiverData);

    public class MNetworkAction
    {
        public int WindowUid;
        public string KeyNetwork;
        public NetworkDelegateTemplate DataDelegate;
        public Dispatcher Dispatcher;

        public MNetworkAction(int WindowUid, NetworkDelegateTemplate DataDelegate, Dispatcher Dispatcher = null, string KeyNetwork = null)
        {
            this.WindowUid = WindowUid;
            this.KeyNetwork = KeyNetwork;
            this.DataDelegate = DataDelegate;
            this.Dispatcher = Dispatcher;
        }
    }
}
