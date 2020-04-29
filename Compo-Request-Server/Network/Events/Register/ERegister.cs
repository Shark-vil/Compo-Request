using Compo_Request_Server.Network.Interfaces.Events;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Shared_Data.Network.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request_Server.Network.Events.Register
{
    public class ERegister : IEvent
    {
        public void Construct()
        {
            NetworkDelegates.Add(RegisterUser, "User.Register", 2);
        }

        public void Destruct()
        {
            throw new NotImplementedException();
        }

        private void RegisterUser(MResponse ClientResponse, UserNetwork UserNetwork)
        {

        }
    }
}
