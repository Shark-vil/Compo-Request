using Compo_Request_Server.Network.Database;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Request_Server.Network.Utilities;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compo_Request_Server.Network.Events.WebRequestActions
{
    public class EWebRequestDir
    {
        public EWebRequestDir()
        {
            NetworkDelegates.Add(WebRequestDirSaver, "WebRequestDir.Save", 1337);
            NetworkDelegates.Add(WebRequestDirDelete, "WebRequestDir.Delete");
        }

        private void WebRequestDirDelete(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            var WebRequestId = Package.Unpacking<int>(ClientResponse.DataBytes);

            using (var db = new DatabaseContext())
            {
                var RequestItem = db.WebRequestItems.FirstOrDefault(x => x.Id == WebRequestId);
                db.WebRequestItems.Remove(RequestItem);
                db.SaveChanges();

                Sender.Broadcast("WebRequestDir.Delete.Confirm", RequestItem, ClientResponse.WindowUid);
            }
        }

        private void WebRequestDirSaver(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            var WebRequestDirItem = Package.Unpacking<WebRequestDir>(ClientResponse.DataBytes);

            using (var db = new DatabaseContext())
            {
                db.Attach(WebRequestDirItem);
                db.SaveChanges();

                Sender.Broadcast("WebRequestDir.Save.Confirm", WebRequestDirItem, ClientResponse.WindowUid);

                WebRequestItem WebRequestItem =
                    db.WebRequestItems.FirstOrDefault(x => x.Id == WebRequestDirItem.WebRequestItemId);

                MBinding_WebRequest MBinding = new MBinding_WebRequest();
                MBinding.Item = WebRequestItem;
                MBinding.Params = db.WebRequestParamsItems.Where(x => x.WebRequestItemId == WebRequestItem.Id).ToArray();
                MBinding.Directory = WebRequestDirItem;

                Sender.Broadcast("WebRequestItem.MBinding_WebRequest.Add", MBinding);
            }
        }
    }
}
