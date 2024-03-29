﻿using Compo_Request.Network.Models;
using Compo_Shared_Data.Debugging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Threading;

namespace Compo_Request.Network.Client
{
    public class NetworkDelegates
    {
        public static List<MNetworkAction> NetworkActions = new List<MNetworkAction>();

        public static void Add(NetworkDelegateTemplate Delegate, Dispatcher Dispatcher = null, int WindowUid = -1, string KeyNetwork = null, string UniqueDelegateName = null, bool NotSingle = false)
        {
            if (NotSingle || UniqueDelegateName == null || !NetworkActions.Exists(x => x.UniqueDelegateName == UniqueDelegateName && x.KeyNetwork == KeyNetwork))
            {
                Debug.Log($"Регистрация делегата: WindowUid - {WindowUid}, KeyNetwork - {KeyNetwork}");
                NetworkActions.Add(new MNetworkAction(WindowUid, Delegate, Dispatcher, KeyNetwork, UniqueDelegateName, NotSingle));
            }
            else
            {
                Debug.Log($"Обновление делегата: WindowUid - {WindowUid}, KeyNetwork - {KeyNetwork}");
                NetworkActions.RemoveAll(x => x.UniqueDelegateName == UniqueDelegateName && x.KeyNetwork == KeyNetwork);
                NetworkActions.Add(new MNetworkAction(WindowUid, Delegate, Dispatcher, KeyNetwork, UniqueDelegateName, NotSingle));
            }
        }

        public static void RemoveByUniqueName(string UniqueDelegateName)
        {
            Debug.Log($"Удаление делегата:  UniqueDelegateName - {UniqueDelegateName}");
            NetworkActions.RemoveAll(x => x.UniqueDelegateName == UniqueDelegateName);
        }
    }
}
