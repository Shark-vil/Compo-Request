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
        public VisualDataDelegate DataDelegate;

        public VisualData(int WindowUid, VisualDataDelegate DataDelegate)
        {
            this.WindowUid = WindowUid;
            this.DataDelegate = DataDelegate;
        }
    }
}
