﻿using Compo_Request_Server.Network.Database;
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
    public class EWebRequestItem
    {
        public EWebRequestItem()
        {
            NetworkDelegates.Add(WebRequestSaver, "WebRequestItem.MBinding_WebRequestSaver.Save");
            NetworkDelegates.Add(WebRequestBindingGet, "WebRequestItem.MBinding_WebRequest.Get");
            NetworkDelegates.Add(WebRequestLinkUpdate, "WebRequestItem.Update.Link");
            NetworkDelegates.Add(WebRequestMethodUpdate, "WebRequestItem.Update.Method");
        }

        private void WebRequestMethodUpdate(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            if (!AccessController.IsPrivilege(NetworkClient, "requests.edit"))
                return;

            using (var db = new DatabaseContext())
            {
                var RequestItem = Package.Unpacking<WebRequestItem>(ClientResponse.DataBytes);

                WebRequestItem DbRequestItem = db.WebRequestItems.FirstOrDefault(x => x.Id == RequestItem.Id);
                DbRequestItem.Method = RequestItem.Method;
                db.SaveChanges();

                Sender.SendOmit(NetworkClient, "WebRequestItem.Update.Method.Confirm", DbRequestItem, ClientResponse.WindowUid);
            }
        }

        private void WebRequestLinkUpdate(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            if (!AccessController.IsPrivilege(NetworkClient, "requests.edit"))
                return;

            using (var db = new DatabaseContext())
            {
                var RequestItem = Package.Unpacking<WebRequestItem>(ClientResponse.DataBytes);

                WebRequestItem DbRequestItem = db.WebRequestItems.FirstOrDefault(x => x.Id == RequestItem.Id);
                DbRequestItem.Link = RequestItem.Link;
                db.SaveChanges();

                Sender.SendOmit(NetworkClient, "WebRequestItem.Update.Link.Confirm", DbRequestItem, ClientResponse.WindowUid);
            }
        }

        private void WebRequestBindingGet(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            if (!AccessController.IsPrivilege(NetworkClient, "requests"))
                return;

            int ProjectId = Package.Unpacking<int>(ClientResponse.DataBytes);

            using (var db = new DatabaseContext())
            {
                List<MBinding_WebRequest> MB_WebRequests = new List<MBinding_WebRequest>();

                WebRequestItem[] WebRequestItems = 
                    db.WebRequestItems.Where(x => x.ProjectId == ProjectId).ToArray();

                foreach (var RequestItem in WebRequestItems)
                {
                    MBinding_WebRequest ListItem = new MBinding_WebRequest();
                    ListItem.Item = RequestItem;
                    ListItem.Params = db.WebRequestParamsItems.Where(x => x.WebRequestItemId == RequestItem.Id).ToArray();
                    ListItem.Directory = db.WebRequestDirs.FirstOrDefault(x => x.WebRequestItemId == RequestItem.Id);

                    MB_WebRequests.Add(ListItem);
                }

                Sender.Send(NetworkClient, "WebRequestItem.MBinding_WebRequest.Get", 
                    MB_WebRequests.ToArray(), ClientResponse.WindowUid);
            }
        }

        private void WebRequestSaver(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            if (!AccessController.IsPrivilege(NetworkClient, "requests.add"))
                return;

            var WebRequestBinding = Package.Unpacking<MBinding_WebRequestSaver>(ClientResponse.DataBytes);

            using (var db = new DatabaseContext())
            {
                WebRequestItem _WebRequestItem = WebRequestBinding.Item;

                db.Attach(_WebRequestItem);
                db.SaveChanges();

                WebRequestParamsItem[] _WebRequestParams = WebRequestBinding.Params;

                for (int i = 0; i < _WebRequestParams.Length; i++)
                {
                    _WebRequestParams[i].WebRequestItemId = _WebRequestItem.Id;
                    
                    _WebRequestParams[i].Description = 
                        (_WebRequestParams[i].Description == null) ? "" : _WebRequestParams[i].Description;

                    db.Attach(_WebRequestParams[i]);
                }
                db.SaveChanges();

                Sender.Broadcast("WebRequestItem.MBinding_WebRequestSaver.Save.Confirm", new MBinding_WebRequestSaver
                {
                    Item = _WebRequestItem,
                    Params = _WebRequestParams
                }, ClientResponse.WindowUid);
            }
        }
    }
}
