﻿using Compo_Request_Server.Network.Database;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
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
                        if (DbUserPrivilege.Privilege == "admin")
                            return true;
                }
            }
            catch { }

            Debug.LogWarning($"Ошибка доступа! Пользователь не имеет необходимых привилегий!");

            return false;
        }

        public static bool IsPrivilege(MNetworkClient NetworkClient, string PrivilegeKey)
        {
            if (IsOwner(NetworkClient))
                return true;

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

            if (IsPrivilegeTeam(NetworkClient, PrivilegeKey))
                return true;

            return false;
        }

        public static bool IsPrivilege(MNetworkClient NetworkClient, string[] PrivilegeKeys)
        {
            if (IsOwner(NetworkClient))
                return true;

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

            if (IsPrivilegeTeam(NetworkClient, PrivilegeKeys))
                return true;

            return false;
        }

        public static bool IsPrivilegeTeam(MNetworkClient NetworkClient, string PrivilegeKey, int TeamGroupId = 0)
        {
            if (IsOwner(NetworkClient))
                return true;

            try
            {
                using (var db = new DatabaseContext())
                {
                    var User = Users.GetUserById(NetworkClient.Id);

                    Debug.Log($"Проверка прав доступа пользователя [{User.Id}] - {User.Login} в командах", ConsoleColor.Gray);

                    TeamPrivilege[] DbUserPrivileges;

                    if (TeamGroupId == 0)
                        DbUserPrivileges = db.TeamPrivileges.ToArray();
                    else
                        DbUserPrivileges = db.TeamPrivileges.Where(x => x.TeamGroupId == TeamGroupId).ToArray();

                    foreach (var DbUserPrivilege in DbUserPrivileges)
                        if (DbUserPrivilege.Privilege == PrivilegeKey || DbUserPrivilege.Privilege == "admin")
                        {
                            Debug.Log($"Привелигия {PrivilegeKey} найдена! Проверка на наличие пользователя в команде...");

                            var DbTeamGroup = db.TeamUsers.FirstOrDefault(x => x.TeamGroupId == DbUserPrivilege.TeamGroupId
                                && x.UserId == User.Id);

                            if (DbTeamGroup != null)
                            {
                                Debug.Log($"> Привилегия [{PrivilegeKey}] доступна", ConsoleColor.Gray);
                                return true;
                            }
                        }

                    Debug.LogWarning($"> Привилегия [{PrivilegeKey}] не доступна в командах");
                }
            }
            catch { }

            Debug.LogWarning($"Ошибка доступа! Пользователь не состоит в команде с необходимыми привилегиями!");

            return false;
        }

        public static bool IsPrivilegeTeam(MNetworkClient NetworkClient, string[] PrivilegeKeys, int TeamGroupId = 0)
        {
            if (IsOwner(NetworkClient))
                return true;

            try
            {
                using (var db = new DatabaseContext())
                {
                    var User = Users.GetUserById(NetworkClient.Id);

                    Debug.Log($"Проверка прав доступа пользователя [{User.Id}] - {User.Login} в командах", ConsoleColor.Gray);

                    TeamPrivilege[] DbUserPrivileges;

                    if (TeamGroupId == 0)
                        DbUserPrivileges = db.TeamPrivileges.ToArray();
                    else
                        DbUserPrivileges = db.TeamPrivileges.Where(x => x.TeamGroupId == TeamGroupId).ToArray();

                    int PCount = 0;

                    foreach (var DbUserPrivilege in DbUserPrivileges)
                        foreach(var PrivilegeKey in PrivilegeKeys)
                            if (DbUserPrivilege.Privilege == PrivilegeKey || DbUserPrivilege.Privilege == "admin")
                            {
                                Debug.Log($"Привелигия {PrivilegeKey} найдена! Проверка на наличие пользователя в команде...");

                                var DbTeamGroup = db.TeamUsers.FirstOrDefault(x => x.TeamGroupId == DbUserPrivilege.TeamGroupId
                                    && x.UserId == User.Id);

                                if (DbTeamGroup != null)
                                {
                                    Debug.Log($"> Привилегия [{PrivilegeKey}] доступна", ConsoleColor.Gray);

                                    if (DbUserPrivilege.Privilege == "admin")
                                        return true;
                                    else
                                        PCount++;
                                }
                            }

                    Debug.Log($"> Проверка соотношения: {PCount} - {PrivilegeKeys.Length}", ConsoleColor.Gray);

                    if (PCount == PrivilegeKeys.Length)
                        return true;

                    Debug.LogWarning($"Ошибка доступа! Команда не имеет необходимых привилегий для пользователя!");
                }
            }
            catch { }

            Debug.LogWarning($"Ошибка доступа! Пользователь не состоит в команде с необходимыми привилегиями!");

            return false;
        }
    }
}
