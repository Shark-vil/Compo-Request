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
        public void Process()
        {
            while (true)
            {
                try
                {
                    byte[] Data = GetRequest();
                    Receiver receiver = Package.Unpacking<Receiver>(Data);

                    bool isBreak = false;

                    foreach (var VData in NetworkDelegates.VisualDataList)
                    {
                        if (VData.Dispatcher != null && VData.WindowUid != -1)
                        {
                            VData.Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                            (ThreadStart)delegate ()
                            {
                                if (VData.WindowUid == receiver.WindowUid)
                                {
                                    VData.DataDelegate(receiver);
                                    isBreak = true;
                                }
                            });
                        }
                        else
                        {
                            VData.DataDelegate(receiver);
                            isBreak = true;
                        }

                        if (isBreak)
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Подключение прервано!");
                    Console.ReadLine();

                    Disconnect();
                }
            }
        }

        private byte[] GetRequest()
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

        static void Disconnect()
        {
            if (ClientNetwork != null)
                ClientNetwork.Close(); //отключение клиента
        }
    }
}
