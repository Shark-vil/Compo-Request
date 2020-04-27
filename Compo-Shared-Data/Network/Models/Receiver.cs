using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.Network.Models
{
    [Serializable]
    public class Receiver
    {
        public string KeyNetwork;
        public byte[] DataBytes;
    }
}
