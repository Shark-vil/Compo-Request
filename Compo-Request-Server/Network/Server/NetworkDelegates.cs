using Compo_Request_Server.Network.Models;
using Compo_Shared_Data.Debugging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request_Server.Network.Server
{
    public class NetworkDelegates
    {
        public static List<MNetworkAction> NetworkActions = new List<MNetworkAction>();

        public static void Add(NetworkDelegateTemplate Delegate, string KeyNetwork = null, int WindowUid = -1)
        {
            Debug.Log($"Регистрация делегата: WindowUid - {WindowUid}, KeyNetwork - {KeyNetwork}");

            NetworkActions.Add(new MNetworkAction(Delegate, KeyNetwork, WindowUid));
        }
    }
}
