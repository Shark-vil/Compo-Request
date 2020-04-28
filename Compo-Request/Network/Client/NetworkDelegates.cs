using Compo_Request.Network.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace Compo_Request.Network.Client
{
    public class NetworkDelegates
    {
        public static List<VisualData> VisualDataList = new List<VisualData>();

        public static void Add(VisualDataDelegate Delegate , int WindowUid = -1, Dispatcher Dispatcher = null, string KeyNetwork = null)
        {
            VisualDataList.Add(new VisualData(WindowUid, Delegate, Dispatcher, KeyNetwork));
        }
    }
}
