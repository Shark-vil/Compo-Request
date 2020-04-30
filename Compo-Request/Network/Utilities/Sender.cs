using Compo_Shared_Data.Network;
using System;
using System.Collections.Generic;
using System.Text;
using Compo_Shared_Data.Network.Models;
using System.Net.Sockets;
using Compo_Shared_Data.Debugging;

namespace Compo_Request.Network.Utilities
{
    public class Sender : NetworkBase
    {
        public static bool SendToServer(string KeyNetwork, object DataObject = null, int WindowUid = -1)
        {
            try
            {
                Debug.Log("Подготовка запроса для отправки на сервер. Информация о запросе: \n" +
                    $"KeyNetwork - {KeyNetwork}, WindowUid - {WindowUid}");

                if (!ClientNetwork.Connected)
                {
                    Debug.LogWarning("Не удалось проверить соединение с сервером, запрос отклонён!");
                    return false;
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
                    ClientNetwork.Send(WriteDataBytes);
                }
                catch(SocketException ex)
                {
                    Debug.LogError("Возникла ошибка при попытке отправить запрос на сервер. " +
                        "Код ошибки:\n" + ex);
                }

                return true;
            }
            catch (Exception ex)
            {
                Debug.LogError("Возникла ошибка при создании экземпляра транспортировки. " +
                    "Код ошибки:\n" + ex);
            }

            return false;
        }
    }
}
