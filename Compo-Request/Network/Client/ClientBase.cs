using Compo_Shared_Data.Network;
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
        public TextBox mainWindow;
        public Dispatcher dispatcher;

        public ClientBase(TextBox mainWindow, Dispatcher dispatcher)
        {
            this.mainWindow = mainWindow;
            this.dispatcher = dispatcher;
        }

        public void Process()
        {
            while (true)
            {
                try
                {
                    byte[] Data = GetRequest();

                    string get = Package.Unpacking<string>(Data);

                    dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (ThreadStart)delegate ()
                        {
                            mainWindow.Text = get;
                        }
                    );
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
