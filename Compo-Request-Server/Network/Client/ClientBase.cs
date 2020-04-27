using System;
using System.Collections.Generic;
using System.Text;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Shared_Data.Network;

namespace Compo_Request_Server.Network.Client
{
    public class ClientBase
    {
        public UserNetwork UserNetwork;
        public ServerBase Server;

        public ClientBase(UserNetwork UserNetwork, ServerBase Server)
        {
            this.UserNetwork = UserNetwork;
            this.Server = Server;

            this.Server.AddConnection(UserNetwork);
        }

        public void Process()
        {
            try
            {
                while (true)
                {
                    try
                    {
                        byte[] Data = GetRequest();

                        Console.WriteLine(Package.Unpacking<string>(Data));

                        UserNetwork.ClientNetwork.Send(Data);
                    }
                    catch
                    {
                        Console.WriteLine("Пользователь отключился.");
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                Server.RemoveConnection(UserNetwork.Id);
                Close();
            }
        }

        private byte[] GetRequest()
        {
            int ByteCount;
            byte[] Bytes;

            do
            {
                Bytes = new byte[UserNetwork.ClientNetwork.ReceiveBufferSize];
                ByteCount = UserNetwork.ClientNetwork.Receive(Bytes);
            }
            while (UserNetwork.ClientNetwork.Available > 0);

            return Bytes;
        }

        protected internal void Close()
        {
            if (UserNetwork.ClientNetwork != null)
                UserNetwork.ClientNetwork.Close();
        }
    }
}
