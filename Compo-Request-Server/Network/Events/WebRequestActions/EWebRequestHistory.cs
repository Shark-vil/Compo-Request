using Compo_Request_Server.Network.Database;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Request_Server.Network.Utilities;
using Compo_Shared_Data.Debugging;
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
    public class EWebRequestHistory
    {
        public EWebRequestHistory()
        {
            NetworkDelegates.Add(HistoryGetAll, "RequestsHistory.GetAll");
            NetworkDelegates.Add(AddHistoryItem, "RequestsHistory.Add");
        }

        private void AddHistoryItem(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            if (!AccessController.IsPrivilege(NetworkClient, "requests.history.add"))
                return;

            try
            {
                var WebRequestHistoryItem = Package.Unpacking<WebRequestHistory>(ClientResponse.DataBytes);

                using (var db = new DatabaseContext())
                {
                    if (WebRequestHistoryItem != null)
                    {
                        db.WebRequestsHistory.Add(WebRequestHistoryItem);
                        db.SaveChanges();

                        Sender.Send(NetworkClient, "RequestsHistory.Add.Confirm", WebRequestHistoryItem);
                    }
                }
            }
            catch (DbException ex)
            {
                Debug.LogError("Возникло исключение при добавлении объекта истории в базу данных. Код ошибки: " + ex);
            }
        }

        private void HistoryGetAll(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            if (!AccessController.IsPrivilege(NetworkClient, "requests.history"))
                return;

            try
            {
                var ProjectId = Package.Unpacking<int>(ClientResponse.DataBytes);

                using(var db = new DatabaseContext())
                {
                    WebRequestHistory[] DbHistory = db.WebRequestsHistory.Where(x => x.ProjectId == ProjectId).ToArray();

                    if (DbHistory.Length != 0)
                    {
                        Sender.Send(NetworkClient, "RequestsHistory.GetAll.Confirm", DbHistory);
                    }
                }
            }
            catch(DbException ex)
            {
                Debug.LogError("Возникло исключение при получении истории запросов с сервера. Код ошибки: " + ex);
            }
        }
    }
}
