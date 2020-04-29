using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request_Server.Network.Interfaces.Events
{
    public interface IEvent
    {
        void Construct();
        void Destruct();
    }
}
