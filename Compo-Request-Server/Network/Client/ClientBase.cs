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
                        MResponse ClientResponse = Package.Unpacking<MResponse>(Data);

                        bool isBreak = false;

                        Debug.Log($"Новый запрос от клиента [{UserNetwork.Id} - {UserNetwork.Ip}:{UserNetwork.NetPoint}]: " +
                            $"WindowUid - {ClientResponse.WindowUid}, KeyNetwork - {ClientResponse.KeyNetwork}");

                        foreach (var DataDelegate in NetworkDelegates.NetworkActions)
                        {
                            if (DataDelegate.WindowUid != -1)
                            {
                                if (DataDelegate.WindowUid == ClientResponse.WindowUid)
                                {
                                    if (CheckKeyNetwork(DataDelegate, ClientResponse))
                                    {
                                        DataDelegate.DataDelegate(ClientResponse, UserNetwork);
                                        isBreak = true;
                                    }
                                }
                            }
                            else
                            {
                                if (CheckKeyNetwork(DataDelegate, ClientResponse))
                                {
                                    DataDelegate.DataDelegate(ClientResponse, UserNetwork);
                                    isBreak = true;
                                }
                            }

                            if (isBreak)
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Log($"Возникла ошибка при обработке пользовательского запроса.\n" +
                            $"Информация о пользователе: [{UserNetwork.Id}] {UserNetwork.Ip}:{UserNetwork.Port}\n" +
                            $"Код ошибки:\n" + ex);

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Возникла ошибка при обработке пользовательского процесса.\n" +
                    $"Информация о пользователе: [{UserNetwork.Id}] {UserNetwork.Ip}:{UserNetwork.Port}\n" +
                    $"Код ошибки:\n" + ex);
            }
            finally
            {
                Server.RemoveConnection(UserNetwork.Id);
                Close();

                Debug.Log($"Пользовательский процесс завершён.\n" +
                    $"Информация о пользователе: [{UserNetwork.Id}] {UserNetwork.Ip}:{UserNetwork.Port}\n");
            }
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
