using Compo_Request_Server.Network.Models;
using Compo_Shared_Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request_Server.Network.Server
{
    public class NetworkDelegates
    {
        public static List<ServerVisualData> VisualDataList = new List<ServerVisualData>();

        public static void Add(ServerVisualDataDelegate Delegate, string KeyNetwork = null, int WindowUid = -1, string UserUid = null)
        {
            VisualDataList.Add(new ServerVisualData(Delegate, KeyNetwork, WindowUid, UserUid));
        }
    }
}
