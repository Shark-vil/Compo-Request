using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.Network.Models
{
    [Serializable]
    public class Receiver
    {
        public string KeyNetwork;
        public string UserUid;
        public int WindowUid = -1;
        public byte[] DataBytes;
    }
}
