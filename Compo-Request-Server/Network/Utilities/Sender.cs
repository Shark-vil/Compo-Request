using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Compo_Request_Server.Network.Models;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;

namespace Compo_Request_Server.Network.Utilities
{
    public class Sender : NetworkBase
    {
        public static void Send(UserNetwork UserNetwork, string KeyNetwork, object DataObject = null, int WindowUid = -1)
        {
            try
            {
                Debug.Log("Подготовка запроса для отправки клиенту. Информация о запросе: \n" +
                    $"KeyNetwork - {KeyNetwork}, WindowUid - {WindowUid}\n" +
                    $"Информация о пользователе: [{UserNetwork.Id}] {UserNetwork.Ip}:{UserNetwork.Port}");

                if (!UserNetwork.ClientNetwork.Connected)
                {
                    Debug.LogWarning("Не удалось проверить соединение с клиентом, запрос отклонён!");
                    return;
                }

                byte[] DataBytes;

                if (DataObject != null && DataObject.GetType().Name == "Byte[]")
                    DataBytes = (byte[])DataObject;
                else
                    DataBytes = Package.Packaging((DataObject == null) ? "" : DataObject);

                var Receiver = new MResponse();
                Receiver.WindowUid = WindowUid;
                Receiver.KeyNetwork = KeyNetwork;
                Receiver.DataBytes = DataBytes;

                byte[] WriteDataBytes = Package.Packaging(Receiver);

                try
                {
                    UserNetwork.ClientNetwork.Send(WriteDataBytes);
                }
                catch (SocketException ex)
                {
                    Debug.LogError("Возникла ошибка при попытке отправить запрос клиенту. " +
                        "Код ошибки:\n" + ex);
                }
            } 
            catch (Exception ex)
            {
                Debug.LogError("Возникла ошибка при создании экземпляра транспортировки. " +
                    "Код ошибки:\n" + ex);
            }
        }

        public static void Broadcast(string KeyNetwork, object DataObject = null, int WindowUid = -1)
        {
            foreach(var UserNetwork in UsersNetwork)
            {
                Send(UserNetwork, KeyNetwork, DataObject, WindowUid);
            }
        }
    }
}
