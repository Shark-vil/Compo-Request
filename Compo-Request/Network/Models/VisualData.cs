using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;
using Compo_Shared_Data.Network.Models;

namespace Compo_Request.Network.Models
{
    public delegate void VisualDataDelegate(Receiver ReceiverData);

    public class VisualData
    {
        public int WindowUid;
        public string KeyNetwork;
        public VisualDataDelegate DataDelegate;
        public Dispatcher Dispatcher;

        public VisualData(int WindowUid, VisualDataDelegate DataDelegate, Dispatcher Dispatcher = null, string KeyNetwork = null)
        {
            this.WindowUid = WindowUid;
            this.KeyNetwork = KeyNetwork;
            this.DataDelegate = DataDelegate;
            this.Dispatcher = Dispatcher;
        }
    }
}
