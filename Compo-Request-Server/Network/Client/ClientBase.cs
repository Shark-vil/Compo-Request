using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Request_Server.Network.Utilities;
using Compo_Shared_Data.Debugging;
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
                        Receiver ServerResponse = Package.Unpacking<Receiver>(Data);

                        bool isBreak = false;

                        Console.WriteLine("Новое сообщение от клиента");

                        foreach (var DataDelegate in NetworkDelegates.VisualDataList)
                        {
                            if (DataDelegate.WindowUid != -1)
                            {
                                if (DataDelegate.WindowUid == ServerResponse.WindowUid)
                                {
                                    if (CheckKeyNetwork(DataDelegate, ServerResponse))
                                    {
                                        // ВТОРОЙ АРГУМЕНТ - ПОЛЬЗОВАТЕЛЬ
                                        DataDelegate.DataDelegate(ServerResponse);
                                        isBreak = true;
                                    }
                                }
                            }
                            else
                            {
                                if (CheckKeyNetwork(DataDelegate, ServerResponse))
                                {
                                    DataDelegate.DataDelegate(ServerResponse);
                                    isBreak = true;
                                }
                            }

                            if (isBreak)
                                break;
                        }
                    }
                    catch
                    {
                        Debug.Log($"The user has completed the local process.\n" +
                            $"User info: [{UserNetwork.Id}] {UserNetwork.Ip}:{UserNetwork.Port}\n");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("The local user process was aborted with an error:\n" + ex + "\n" +
                    $"User info: [{UserNetwork.Id}] {UserNetwork.Ip}:{UserNetwork.Port}\n");
            }
            finally
            {
                Server.RemoveConnection(UserNetwork.Id);
                Close();

                Debug.Log($"Final completion of the user process.\n" +
                    $"User info: [{UserNetwork.Id}] {UserNetwork.Ip}:{UserNetwork.Port}\n");
            }
        }

        private static bool CheckKeyNetwork(ServerVisualData DataDelegate, Receiver ServerResponse)
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
