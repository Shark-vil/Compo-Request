using Compo_Shared_Data.Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;

namespace Compo_Request.Network.Client
{
    public class ClientBase : NetworkBase
    {
        public TextBox mainWindow;

        public ClientBase(TextBox mainWindow)
        {
            this.mainWindow = mainWindow;
        }

        public void Process()
        {
            while (true)
            {
                try
                {
                    byte[] Data = GetRequest();

                    string get = Package.Unpacking<string>(Data);

                    Console.WriteLine(get);

                    mainWindow.Text = get;
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
