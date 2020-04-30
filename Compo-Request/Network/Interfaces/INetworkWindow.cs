using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request.Network.Interfaces
{
    public interface INetworkWindow
    {
        void LoadDelegates();
        void UnloadDelegates();
        void EventsInitialize();
    }
}
