using Compo_Request_Server.Network.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request_Server.Network.Server
{
    public class NetworkDelegates
    {
        public static List<MNetworkAction> VisualDataList = new List<MNetworkAction>();

        public static void Add(NetworkDelegateTemplate Delegate, string KeyNetwork = null, int WindowUid = -1, string UserUid = null)
        {
            VisualDataList.Add(new MNetworkAction(Delegate, KeyNetwork, WindowUid, UserUid));
        }
    }
}
