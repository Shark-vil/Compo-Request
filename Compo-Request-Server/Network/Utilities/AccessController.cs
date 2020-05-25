using Compo_Request_Server.Network.Database;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compo_Request_Server.Network.Utilities
{
    public class AccessController
    {
        public static bool IsOwner(MNetworkClient NetworkClient)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var User = Users.GetUserById(NetworkClient.Id);

                    if (User.Id == 1)
                        return true;

                    var DbUserPrivileges = db.UserPrivileges.Where(x => x.UserId == User.Id).ToArray();

                    foreach(var DbUserPrivilege in DbUserPrivileges)
                        if (DbUserPrivilege.Privilege == "owner")
                            return true;
                }
            }
            catch { }

            return false;
        }

        public static bool IsPrivilege(MNetworkClient NetworkClient, string PrivilegeKey)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var User = Users.GetUserById(NetworkClient.Id);

                    var DbUserPrivileges = db.UserPrivileges.Where(x => x.UserId == User.Id).ToArray();

                    foreach (var DbUserPrivilege in DbUserPrivileges)
                        if (DbUserPrivilege.Privilege == PrivilegeKey)
                            return true;
                }
            }
            catch { }

            return false;
        }

        public static bool IsPrivilege(MNetworkClient NetworkClient, string[] PrivilegeKeys)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var User = Users.GetUserById(NetworkClient.Id);

                    var DbUserPrivileges = db.UserPrivileges.Where(x => x.UserId == User.Id).ToArray();

                    foreach (var PrivilegeKey in PrivilegeKeys)
                        if (!Array.Exists(DbUserPrivileges, x => x.Privilege == PrivilegeKey))
                            return false;

                    return true;
                }
            }
            catch { }

            return false;
        }
    }
}
