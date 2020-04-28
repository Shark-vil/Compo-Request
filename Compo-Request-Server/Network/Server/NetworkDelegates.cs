using Compo_Shared_Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request_Server.Network.Server
{
    public class NetworkDelegates
    {
        public static List<VisualData> VisualDataList = new List<VisualData>();

        public static void Add(VisualDataDelegate Delegate, int WindowUid = -1)
        {
            VisualDataList.Add(new VisualData(WindowUid, Delegate));
        }
    }
}
