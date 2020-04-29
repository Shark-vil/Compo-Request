using Compo_Request.Network.Models;
using Compo_Shared_Data.Debugging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Threading;

namespace Compo_Request.Network.Client
{
    public class NetworkDelegates
    {
        public static List<MNetworkAction> NetworkActions = new List<MNetworkAction>();

        public static void Add(NetworkDelegateTemplate Delegate, Dispatcher Dispatcher = null, int WindowUid = -1, string KeyNetwork = null)
        {
            Debug.Log($"Delegate Registration: WindowUid - {WindowUid}, KeyNetwork - {KeyNetwork}");

            NetworkActions.Add(new MNetworkAction(WindowUid, Delegate, Dispatcher, KeyNetwork));
        }
    }
}
