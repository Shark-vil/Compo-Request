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
    public class EWebRequestParamsItem
    {
        public EWebRequestParamsItem()
        {
            NetworkDelegates.Add(WebRequestParamsGet, "WebRequestParamsItem.Get");
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
