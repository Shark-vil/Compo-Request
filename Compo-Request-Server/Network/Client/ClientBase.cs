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
                byte[] Data = GetRequest();

                Console.WriteLine(Package.Unpacking<string>(Data));
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

        private  byte[] GetRequest()
        {
            byte[] bytes;

            do
            {
                bytes = new byte[UserNetwork.ClientNetwork.ReceiveBufferSize];
                UserNetwork.Stream.Read(bytes, 0, (int)UserNetwork.ClientNetwork.ReceiveBufferSize);
            }
            while (UserNetwork.Stream.DataAvailable);

            return bytes;
        }

        protected internal void Close()
        {
            if (UserNetwork.Stream != null)
                UserNetwork.Stream.Close();

            if (UserNetwork.ClientNetwork != null)
                UserNetwork.ClientNetwork.Close();
        }
    }
}
