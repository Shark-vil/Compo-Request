using Compo_Shared_Data.Network.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.Models
{
    public delegate void VisualDataDelegate(Receiver ReceiverData);

    public class VisualData
    {
        public int WindowUid;
        public int ClientCode;
        public VisualDataDelegate DataDelegate;

        public VisualData(VisualDataDelegate DataDelegate, int WindowUid = -1, int ClientCode = -1)
        {
            this.WindowUid = WindowUid;
            this.ClientCode = ClientCode;
            this.DataDelegate = DataDelegate;
        }
    }
}
