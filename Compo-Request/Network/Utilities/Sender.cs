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
        public static void SendToServer(string KeyNetwork, object DataObject = null, int WindowUid = -1)
        {
            try
            {
                byte[] DataBytes;

                if (DataObject.GetType().Name == "Byte[]")
                    DataBytes = (byte[])DataObject;
                else
                    DataBytes = Package.Packaging(DataObject);

                var Receiver = new Receiver();
                Receiver.WindowUid = WindowUid;
                Receiver.KeyNetwork = KeyNetwork;
                Receiver.DataBytes = DataBytes;

                byte[] WriteDataBytes = Package.Packaging(Receiver);

                ClientNetwork.Send(WriteDataBytes);
            }
            catch (SocketException ex)
            {
                Debug.LogError("[Sender.Send] Socket exception:\n" + ex);
            }
        }
    }
}
