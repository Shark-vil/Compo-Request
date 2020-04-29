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
                if (!UserNetwork.ClientNetwork.Connected)
                {
                    Debug.LogWarning("Failed to send message to client!\n" +
                        $"[{KeyNetwork}] WindowUid - {WindowUid}\n" +
                        $"User info: [{UserNetwork.Id}] {UserNetwork.Ip}:{UserNetwork.Port}\n");
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
                catch (Exception ex)
                {
                    Debug.LogError("An exception was thrown when sending a request to the user. " +
                        "Exception code:\n" + ex);
                }
            } 
            catch (SocketException ex)
            {
                Debug.LogError("[Sender.Send] Socket exception:\n" + ex);
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
