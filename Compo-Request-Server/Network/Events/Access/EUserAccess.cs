﻿using Compo_Request_Server.Network.Database;
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
            NetworkDelegates.Add(GetAccessList, "Access.GetAll");
            NetworkDelegates.Add(AccessGetAll, "User.Access.GetAll");
            NetworkDelegates.Add(AccessUpdate, "User.Access.Update");
        }

        private void GetAccessList(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            List<MAccess> Access = new List<MAccess>()
            {
                new MAccess { Key = "admin", Description = "Полный доступ" },

                new MAccess { Key = "users", Description = "Доступ к списку пользователей" },
                new MAccess { Key = "users.add", Description = "Добавление пользователей" },
                new MAccess { Key = "users.edit", Description = "Редактирование пользователей" },
                new MAccess { Key = "users.delete", Description = "Удаление пользователей" },

                new MAccess { Key = "teams", Description = "Доступ к списку команд" },
                new MAccess { Key = "teams.add", Description = "Добавление команд" },
                new MAccess { Key = "teams.edit", Description = "Редактирование команд" },
                new MAccess { Key = "teams.delete", Description = "Удаление команд" },

                new MAccess { Key = "projects", Description = "Доступ к списку проектов" },
                new MAccess { Key = "projects.add", Description = "Добавление проектов" },
                new MAccess { Key = "projects.edit", Description = "Редактирование проектов" },
                new MAccess { Key = "projects.delete", Description = "Удаление проектов" },
                new MAccess { Key = "projects.chat", Description = "Доступ к чату проекта" },

                new MAccess { Key = "requests", Description = "Доступ к списку WEB-запросов" },
                new MAccess { Key = "requests.add", Description = "Добавление WEB-запросов" },
                new MAccess { Key = "requests.edit", Description = "Редактирование WEB-запросов" },
                new MAccess { Key = "requests.delete", Description = "Удаление WEB-запросов" },
                new MAccess { Key = "requests.history", Description = "Доступ к истории WEB-запросов" },
            };

            Sender.Send(NetworkClient, "Access.GetAll", Access.ToArray());
        }

        private void AccessGetAll(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var User = Users.GetUserById(NetworkClient.Id);
                    var DbUserPrivileges = db.UserPrivileges.Where(x => x.UserId == User.Id).ToArray();

                    Debug.Log($"Получен список прав доступа в количестве {DbUserPrivileges.Length} записей.", ConsoleColor.Magenta);

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

                    Debug.Log($"Обновление прав доступа пользователя.\n", ConsoleColor.Magenta);

                    List <UserPrivilege> DataRemoves = new List<UserPrivilege>();

                    Debug.Log($"Список прав на удаление:\n", ConsoleColor.Magenta);
                    foreach (var UserPrivilege in DbUserPrivileges)
                        if (!Array.Exists(ReadUserPrivileges, x => x.Privilege == UserPrivilege.Privilege))
                        {
                            DataRemoves.Add(UserPrivilege);
                            Debug.Log($"> Privilege - {UserPrivilege.Privilege}\n", ConsoleColor.Magenta);
                        }

                    Debug.Log($"Список прав на добавление:\n", ConsoleColor.Magenta);
                    foreach (var UserPrivilege in ReadUserPrivileges)
                    {
                        db.UserPrivileges.Add(UserPrivilege);
                        Debug.Log($"> Privilege - {UserPrivilege.Privilege}\n", ConsoleColor.Magenta);
                    }

                    foreach (var UserPrivilege in DataRemoves)
                        db.UserPrivileges.Remove(UserPrivilege);

                    db.SaveChanges();

                    Debug.Log($"Права пользователя [{User.Id}] - {User.Login} обновлены!\n", ConsoleColor.Magenta);

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
