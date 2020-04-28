using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Request_Server.Network.Utilities;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;

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
                        Receiver receiver = Package.Unpacking<Receiver>(Data);

                        bool isBreak = false;

                        foreach (var VData in NetworkDelegates.VisualDataList)
                        {
                            if (VData.WindowUid != -1)
                            {
                                if (VData.WindowUid == receiver.WindowUid)
                                {
                                    VData.DataDelegate(receiver);
                                    isBreak = true;
                                }
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
