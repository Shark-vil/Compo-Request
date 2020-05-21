using Compo_Request_Server.Network.Database;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Request_Server.Network.Utilities;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using Compo_Shared_Data.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Compo_Request_Server.Network.Events.WebRequestActions
{
    public class EWebRequestDir
    {
        public EWebRequestDir()
        {
            NetworkDelegates.Add(WebRequestDirSaver, "WebRequestDir.Save", 1337);
            NetworkDelegates.Add(WebRequestDirUpdate, "WebRequestDir.RequestDirectory.Update", 1337);
            NetworkDelegates.Add(WebRequestDirDelete, "WebRequestDir.Delete");
            NetworkDelegates.Add(WebRequestDirHistoryEdit, "WebRequestDir.History.Edit");
        }

        private void WebRequestDirHistoryEdit(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            var RequestHistory = Package.Unpacking<WebRequestHistory>(ClientResponse.DataBytes);

            using (var db = new DatabaseContext())
            {
                ModelRequestDirectory RequestDir = new ModelRequestDirectory();
                RequestDir.RequestMethod = RequestHistory.Method;
                RequestDir.RequestTitle = RequestHistory.Title;
                RequestDir.WebRequestId = RequestHistory.Id;

                WebRequestDir WebDir = db.WebRequestDirs.FirstOrDefault(x => x.WebRequestItemId == RequestHistory.WebRequestItemId);
                RequestDir.Id = WebDir.Id;
                RequestDir.Title = WebDir.Title;

                Sender.Broadcast("WebRequestDir.History.Edit.Confirm",
                    RequestDir, ClientResponse.WindowUid);
            }
        }

        private void WebRequestDirUpdate(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            var RequestDirectory = Package.Unpacking<ModelRequestDirectory>(ClientResponse.DataBytes);

            using (var db = new DatabaseContext())
            {
                WebRequestDir WebDir = db.WebRequestDirs.FirstOrDefault(x => x.Id == RequestDirectory.Id);
                WebDir.Title = RequestDirectory.Title;

                WebRequestItem RequestItem = db.WebRequestItems.FirstOrDefault(x => x.Id == WebDir.WebRequestItemId);
                RequestItem.Title = RequestDirectory.RequestTitle;

                db.SaveChanges();

                Sender.Broadcast("WebRequestDir.RequestDirectory.Update.Confirm",
                    RequestDirectory, ClientResponse.WindowUid);
            }
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
