using Compo_Request.Network.Models;
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
                    Receiver ServerResponse = Package.Unpacking<Receiver>(Data);

                    Console.WriteLine("Сообщение с сервера");

                    foreach (var DataDelegate in NetworkDelegates.VisualDataList)
                    {
                        if (DataDelegate.Dispatcher != null && DataDelegate.WindowUid != -1)
                        {
                            if (DataDelegate.WindowUid == ServerResponse.WindowUid)
                            {
                                if (CheckKeyNetwork(DataDelegate, ServerResponse))
                                {
                                    DispatcherExec(DataDelegate, ServerResponse);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (CheckKeyNetwork(DataDelegate, ServerResponse))
                            {
                                DataDelegate.DataDelegate(ServerResponse);
                                break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Подключение прервано!");

                    foreach (var DataDelegate in NetworkDelegates.VisualDataList)
                        if (DataDelegate.KeyNetwork == "Server.Disconnect")
                            DispatcherExec(DataDelegate);

                    Disconnect();

                    break;
                }
            }
        }

        private static bool DispatcherExec(VisualData DataDelegate, Receiver ServerResponse = null)
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

            return false;
        }

        private static bool CheckKeyNetwork(VisualData DataDelegate, Receiver ServerResponse)
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
        }
    }
}
