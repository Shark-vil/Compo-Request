﻿using Compo_Request.Network.Models;
using Compo_Request.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Compo_Request.Network.Client
{
    public class ClientBase : NetworkBase
    {
        public static Thread SelfThread;

        public static void Process()
        {
            while (true)
            {
                try
                {
                    byte[] Data = GetRequest();

                    MResponse ServerResponse = Package.Unpacking<MResponse>(Data);

                    Debug.Log($"Новый запрос от сервера [{Host}:{Port}]: " +
                        $"WindowUid - {ServerResponse.WindowUid}, KeyNetwork - {ServerResponse.KeyNetwork}");

                    bool IsCorrectKey = false;

                    foreach (var DataDelegate in NetworkDelegates.NetworkActions)
                    {
                        if (DataDelegate.Dispatcher != null && DataDelegate.WindowUid != -1)
                        {
                            if (DataDelegate.WindowUid == ServerResponse.WindowUid)
                            {
                                if (CheckKeyNetwork(DataDelegate, ServerResponse))
                                {
                                    DispatcherExec(DataDelegate, ServerResponse);
                                    IsCorrectKey = true;

                                    if (!DataDelegate.NotSingle)
                                        break;
                                }
                            }
                        }
                        else
                        {
                            if (CheckKeyNetwork(DataDelegate, ServerResponse))
                            {
                                DispatcherExec(DataDelegate, ServerResponse);
                                IsCorrectKey = true;

                                if (!DataDelegate.NotSingle)
                                    break;
                            }
                        }
                    }

                    if (!IsCorrectKey)
                        Debug.LogWarning("Не найдено делегатов с таким идентификатором!");
                }
                catch (Exception ex)
                {
                    Debug.LogError("Потеряно соединение с сервером! Код ошибки:\n" + ex);

                    foreach (var DataDelegate in NetworkDelegates.NetworkActions)
                        if (DataDelegate.KeyNetwork == "Server.Disconnect")
                            DispatcherExec(DataDelegate);

                    ConnectService.ConnectBrokenEvents.Invoke();

                    Disconnect();

                    break;
                }
            }
        }

        private static bool DispatcherExec(MNetworkAction DataDelegate, MResponse ServerResponse = null)
        {
            try
            {
                if (DataDelegate.Dispatcher != null)
                {

                    DataDelegate.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (ThreadStart)delegate ()
                    {
                        DataDelegate.DataDelegate(ServerResponse);
                    });

                    return true;
                }
            }
            catch(Exception ex)
            {
                Debug.LogError("Возникла ошибка при вызове делегата. Код ошибки:\n" + ex);
            }

            return false;
        }

        private static bool CheckKeyNetwork(MNetworkAction DataDelegate, MResponse ServerResponse)
        {
            if (DataDelegate.KeyNetwork != null)
            {
                if (DataDelegate.KeyNetwork == ServerResponse.KeyNetwork)
                    return true;
            }
            else
                return true;

            return false;
        }

        private static byte[] GetRequest()
        {
            int ByteCount;
            byte[] Bytes;

            do
            {
                Bytes = new byte[ClientNetwork.ReceiveBufferSize];
                ByteCount = ClientNetwork.Receive(Bytes);
            }
            while (ClientNetwork.Available > 0);

            return Bytes;
        }

        public static void Disconnect()
        {
            if (ClientNetwork != null)
                ClientNetwork.Close();

            Debug.Log("Отключение от сервера");
        }
    }
}
