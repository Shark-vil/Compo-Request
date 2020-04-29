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
                if (!ClientNetwork.Connected)
                    return false;

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
                catch(Exception ex)
                {
                    Debug.LogError("An exception was thrown when sending a request to the server. " +
                        "Exception code:\n" + ex);
                }

                return true;
            }
            catch (SocketException ex)
            {
                Debug.LogError("[Sender.Send] Socket exception:\n" + ex);
            }

            return false;
        }
    }
}
