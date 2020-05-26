using Compo_Request_Server.Network.Database;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Request_Server.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Network.Models;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Compo_Request_Server.Network.Events.UserEvents
{
    public class EUsers
    {
        public EUsers()
        {
            NetworkDelegates.Add(UsersGetAll, "Users.GetAll");
        }

        private void UsersGetAll(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            try
            {
                using(var db = new DatabaseContext())
                {
                    var DbUsers = db.Users.ToArray();
                    var WUsers = DbConvertToWpf.ConvertUser(DbUsers);

                    if (AccessController.IsPrivilege(NetworkClient, "users"))
                        Sender.Send(NetworkClient, "Users.GetAll.Confirm", WUsers);
                }
            }
            catch(DbException ex)
            {
                Debug.LogError("Возникло исключение при получении списка пользователей. Код ошибки:\n" + ex);
                Sender.Send(NetworkClient, "Users.GetAll.Error");
            }
        }
    }
}
