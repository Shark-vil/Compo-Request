﻿using Compo_Request_Server.Network.Database;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Request_Server.Network.Utilities;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request_Server.Network.Events.WebRequestActions
{
    public class EWebRequestDir
    {
        public EWebRequestDir()
        {
            NetworkDelegates.Add(WebRequestDirSaver, "WebRequestDir.Save", 1337);
        }

        private void WebRequestDirSaver(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            var WebRequestDirItem = Package.Unpacking<WebRequestDir>(ClientResponse.DataBytes);

            using (var db = new DatabaseContext())
            {
                db.Attach(WebRequestDirItem);
                db.SaveChanges();

                Sender.Broadcast("WebRequestDir.Save.Confirm", WebRequestDirItem, ClientResponse.WindowUid);
            }
        }
    }
}
