using Compo_Request_Server.Network.Database;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Request_Server.Network.Utilities;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Compo_Request_Server.Network.Events.WebRequestActions
{
    public class EWebRequestParamsItem
    {
        public EWebRequestParamsItem()
        {
            NetworkDelegates.Add(WebRequestParamsGet, "WebRequestParamsItem.Get");
            NetworkDelegates.Add(WebRequestParamsUpdate, "WebRequestParamsItem.Update");
            NetworkDelegates.Add(WebRequestParamsDelete, "WebRequestParamsItem.Delete");
            NetworkDelegates.Add(WebRequestParamsDeleteAll, "WebRequestParamsItem.Delete.All");
        }

        private void WebRequestParamsDeleteAll(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            var WebRequestId = Package.Unpacking<int>(ClientResponse.DataBytes);

            using (var db = new DatabaseContext())
            {
                var DbRequestParamsItems = db.WebRequestParamsItems.Where(x => x.WebRequestItemId == WebRequestId).ToArray();

                if (DbRequestParamsItems != null && DbRequestParamsItems.Length != 0)
                {
                    db.WebRequestParamsItems.RemoveRange(DbRequestParamsItems);
                    db.SaveChanges();

                    Sender.Broadcast("WebRequestParamsItem.Delete.All.Confirm", WebRequestId);
                }
            }
        }

        private void WebRequestParamsDelete(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            var WebParamsId = Package.Unpacking<int>(ClientResponse.DataBytes);

            using (var db = new DatabaseContext())
            {
                var DbRequestParamsItem = db.WebRequestParamsItems.FirstOrDefault(x => x.Id == WebParamsId);
                
                if (DbRequestParamsItem != null)
                {
                    db.WebRequestParamsItems.Remove(DbRequestParamsItem);
                    db.SaveChanges();

                    Sender.Broadcast("WebRequestParamsItem.Delete.Confirm", WebParamsId);
                }
            }
        }

        private void WebRequestParamsUpdate(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            try
            {
                var RequestParamsItems = Package.Unpacking<MBinding_WebRequestSaver>(ClientResponse.DataBytes);

                using (var db = new DatabaseContext())
                {
                    var DbRequestParamsItems = new List<WebRequestParamsItem>();

                    foreach (var RequestParamsItem in RequestParamsItems.Params)
                    {
                        WebRequestParamsItem DbRequestParam = db.WebRequestParamsItems.FirstOrDefault
                            (x => x.Id == RequestParamsItem.Id);

                        if (DbRequestParam != null)
                        {
                            DbRequestParam.Key = RequestParamsItem.Key;
                            DbRequestParam.Value = RequestParamsItem.Value;
                            DbRequestParam.Description = RequestParamsItem.Description;
                            db.SaveChanges();
                            DbRequestParamsItems.Add(DbRequestParam);
                        }
                        else
                        {
                            //if (db.WebRequestParamsItems.FirstOrDefault(x => x.Key == RequestParamsItem.Key) == null)
                            //{
                                DbRequestParam = RequestParamsItem;
                                DbRequestParam.Key = (DbRequestParam.Key != null) ? DbRequestParam.Key : "";
                                DbRequestParam.Value = (DbRequestParam.Value != null) ? DbRequestParam.Value : "";
                                DbRequestParam.Description = (DbRequestParam.Description != null) ? DbRequestParam.Description : "";
                                DbRequestParam.WebRequestItemId = RequestParamsItems.Item.Id;

                                if (DbRequestParam.Key.Trim() != string.Empty || DbRequestParam.Value.Trim() != string.Empty)
                                {
                                    db.WebRequestParamsItems.Add(DbRequestParam);
                                    db.SaveChanges();
                                    DbRequestParamsItems.Add(DbRequestParam);
                                }
                            //}
                        }
                    }

                    if (DbRequestParamsItems.Count != 0)
                        Sender.Broadcast("WebRequestParamsItem.Update.Confirm", DbRequestParamsItems.ToArray());
                }
            }
            catch(DbException ex)
            {
                Sender.Send(NetworkClient, "WebRequestParamsItem.Update.Error");
            }
        }

        private void WebRequestParamsGet(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            int ItemId = Package.Unpacking<int>(ClientResponse.DataBytes);

            using (var db = new DatabaseContext())
            {
                WebRequestParamsItem[] RequestParams = db.WebRequestParamsItems.ToArray();

                Sender.Send(NetworkClient, "WebRequestParamsItem.Get.Confirm", RequestParams, ClientResponse.WindowUid);
            }
        }
    }
}
