using Compo_Request_Server.Network.Database;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Shared_Data.Debugging;
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

                    Debug.Log($"Проверка на полный доступ пользователя [{User.Id}] - {User.Login}", ConsoleColor.Gray);

                    if (User.Id == 1)
                        return true;

                    var DbUserPrivileges = db.UserPrivileges.Where(x => x.UserId == User.Id).ToArray();

                    foreach(var DbUserPrivilege in DbUserPrivileges)
                        if (DbUserPrivilege.Privilege == "owner")
                            return true;
                }
            }
            catch { }

            Debug.LogWarning($"Ошибка доступа! Пользователь не имеет необходимых привилегий!");

            return false;
        }

        public static bool IsPrivilege(MNetworkClient NetworkClient, string PrivilegeKey)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var User = Users.GetUserById(NetworkClient.Id);

                    Debug.Log($"Проверка прав доступа пользователя [{User.Id}] - {User.Login}", ConsoleColor.Gray);

                    var DbUserPrivileges = db.UserPrivileges.Where(x => x.UserId == User.Id).ToArray();

                    foreach (var DbUserPrivilege in DbUserPrivileges)
                        if (DbUserPrivilege.Privilege == PrivilegeKey)
                        {
                            Debug.Log($"> Привилегия [{PrivilegeKey}] доступна", ConsoleColor.Gray);
                            return true;
                        }

                    Debug.LogWarning($"> Привилегия [{PrivilegeKey}] не доступна");
                }
            }
            catch { }

            Debug.LogWarning($"Ошибка доступа! Пользователь не имеет необходимых привилегий!");

            return false;
        }

        public static bool IsPrivilege(MNetworkClient NetworkClient, string[] PrivilegeKeys)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var User = Users.GetUserById(NetworkClient.Id);

                    Debug.Log($"Проверка прав доступа пользователя [{User.Id}] - {User.Login}", ConsoleColor.Gray);

                    var DbUserPrivileges = db.UserPrivileges.Where(x => x.UserId == User.Id).ToArray();

                    foreach (var PrivilegeKey in PrivilegeKeys)
                        if (!Array.Exists(DbUserPrivileges, x => x.Privilege == PrivilegeKey))
                        {
                            Debug.LogWarning($"> Привилегия [{PrivilegeKey}] не доступна");
                            return false;
                        }
                        else
                            Debug.Log($"> Привилегия [{PrivilegeKey}] доступна", ConsoleColor.Gray);

                    return true;
                }
            }
            catch { }

            Debug.LogWarning($"Ошибка доступа! Пользователь не имеет необходимых привилегий!");

            return false;
        }
    }
}
