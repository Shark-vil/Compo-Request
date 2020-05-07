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
        public MNetworkClient NetworkClient;
        public ServerBase _ServerBase;

        public ClientBase(MNetworkClient NetworkClient, ServerBase Server)
        {
            this.NetworkClient = NetworkClient;
            this._ServerBase = Server;

            this._ServerBase.AddConnection(NetworkClient);
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

                        //bool isBreak = false;
                        bool IsCorrectKey = false;

                        Debug.Log($"Новый запрос от клиента [{NetworkClient.Id} - {NetworkClient.Ip}:{NetworkClient.NetPoint}]: " +
                            $"WindowUid - {ClientResponse.WindowUid}, KeyNetwork - {ClientResponse.KeyNetwork}");

                        foreach (var DataDelegate in NetworkDelegates.NetworkActions)
                        {
                            if (DataDelegate.WindowUid != -1)
                            {
                                if (DataDelegate.WindowUid == ClientResponse.WindowUid)
                                {
                                    if (CheckKeyNetwork(DataDelegate, ClientResponse))
                                    {
                                        DataDelegate.DataDelegate(ClientResponse, NetworkClient);
                                        IsCorrectKey = true;
                                        //isBreak = true;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                if (CheckKeyNetwork(DataDelegate, ClientResponse))
                                {
                                    DataDelegate.DataDelegate(ClientResponse, NetworkClient);
                                    IsCorrectKey = true;
                                    //isBreak = true;
                                    break;
                                }
                            }

                            /*
                            if (isBreak)
                                break;
                            */
                        }

                        if (!IsCorrectKey)
                            Debug.LogWarning("Не найдено делегатов с таким идентификатором!");
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Возникла ошибка при обработке пользовательского запроса.\n" +
                            $"Информация о пользователе: [{NetworkClient.Id}] {NetworkClient.Ip}:{NetworkClient.Port}\n" +
                            $"Код ошибки:\n" + ex);

                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Возникла ошибка при обработке пользовательского процесса.\n" +
                    $"Информация о пользователе: [{NetworkClient.Id}] {NetworkClient.Ip}:{NetworkClient.Port}\n" +
                    $"Код ошибки:\n" + ex);
            }
            finally
            {
                Server.Users.Remove(NetworkClient);

                _ServerBase.RemoveConnection(NetworkClient.Id);
                Close();

                Debug.Log($"Пользовательский процесс завершён.\n" +
                    $"Информация о пользователе: [{NetworkClient.Id}] {NetworkClient.Ip}:{NetworkClient.Port}\n");
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
                Bytes = new byte[NetworkClient.ClientNetwork.ReceiveBufferSize];
                ByteCount = NetworkClient.ClientNetwork.Receive(Bytes);
            }
            while (NetworkClient.ClientNetwork.Available > 0);

            return Bytes;
        }

        protected internal void Close()
        {
            if (NetworkClient.ClientNetwork != null)
                NetworkClient.ClientNetwork.Close();
        }
    }
}
