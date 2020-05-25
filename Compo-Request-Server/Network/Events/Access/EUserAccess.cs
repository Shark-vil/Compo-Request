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

namespace Compo_Request_Server.Network.Events.Access
{
    public class EUserAccess
    {
        public EUserAccess()
        {
            NetworkDelegates.Add(AccessGetAll, "User.Access.GetAll");
            NetworkDelegates.Add(AccessUpdate, "User.Access.Update");
        }

        private void AccessGetAll(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var User = Users.GetUserById(NetworkClient.Id);
                    var DbUserPrivileges = db.UserPrivileges.Where(x => x.UserId == User.Id).ToArray();

                    Sender.Send(NetworkClient, "User.Access.GetAll.Confirm", 
                        DbUserPrivileges, ClientResponse.WindowUid);
                }
            }
            catch(DbException ex)
            {
                Debug.LogError("Возникло исключение при получении списка прав доступа. Код ошибки:\n" + ex);
                Sender.Send(NetworkClient, "User.Access.GetAll.Error");
            }
        }

        private void AccessUpdate(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var ReadUserPrivileges = Package.Unpacking<UserPrivilege[]>(ClientResponse.DataBytes);

                    var User = Users.GetUserById(NetworkClient.Id);
                    var DbUserPrivileges = db.UserPrivileges.Where(x => x.UserId == User.Id).ToArray();

                    List<UserPrivilege> DataRemoves = new List<UserPrivilege>();
                    foreach(var UserPrivilege in DbUserPrivileges)
                        if (!Array.Exists(ReadUserPrivileges, x => x.Privilege == UserPrivilege.Privilege))
                            DataRemoves.Add(UserPrivilege);

                    foreach (var UserPrivilege in ReadUserPrivileges)
                        db.UserPrivileges.Add(UserPrivilege);

                    foreach (var UserPrivilege in DataRemoves)
                        db.UserPrivileges.Remove(UserPrivilege);

                    db.SaveChanges();

                    DbUserPrivileges = db.UserPrivileges.Where(x => x.UserId == User.Id).ToArray();

                    Sender.Broadcast("User.Access.Update.Confirm", 
                        DbUserPrivileges, ClientResponse.WindowUid);
                }
            }
            catch (DbException ex)
            {
                Debug.LogError("Возникло исключение при обновлении списка прав доступа. Код ошибки:\n" + ex);
                Sender.Send(NetworkClient, "User.Access.Update.Error");
            }
        }
    }
}
