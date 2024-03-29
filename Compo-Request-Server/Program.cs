﻿using System;
using System.Data.Common;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Compo_Request_Server.Network;
using Compo_Request_Server.Network.Database;
using Compo_Request_Server.Network.Events.Access;
using Compo_Request_Server.Network.Events.Auth;
using Compo_Request_Server.Network.Events.Chats;
using Compo_Request_Server.Network.Events.Projects;
using Compo_Request_Server.Network.Events.Register;
using Compo_Request_Server.Network.Events.Team;
using Compo_Request_Server.Network.Events.UserEvents;
using Compo_Request_Server.Network.Events.WebRequestActions;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace Compo_Request_Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Debug.Log("Подготовка сервера...", ConsoleColor.Cyan);

            string[] SqlInfo = Data.Network.SqlData.Read();

            Debug.Log("Настройка базы данных");
            DatabaseContext.Setup(SqlInfo[0], SqlInfo[1], SqlInfo[2], SqlInfo[3]);

            try
            {
                Debug.Log("[DB] Проверка подключения к базе данных...", ConsoleColor.Magenta);
                using (var db = new DatabaseContext())
                {
                    Debug.Log($"[DB] Статус - " +
                        $"{(db.Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator).Exists()}",
                        ConsoleColor.Magenta);
                }
            }
            catch (DbException ex)
            {
                Debug.LogError("Ошибка подключения к базе данных. Код ошибки:\n" + ex);
            }

            var Server = new ServerBase();

            try
            {
                RegisterEvents();

                var ServerThread = new Thread(new ThreadStart(Server.Listen));
                ServerThread.Start();
            }
            catch (Exception ex)
            {
                Debug.LogError("Возникла ошибка при попытке запустить сервер! Код ошибки:\n" + ex);
                ServerBase.Disconnect();
            }
        }

        private static void RegisterEvents()
        {
            var EventRegister = new ERegister();
            var EventAuth = new EAuth();
            var EventTeamGroup = new ETeam();
            var EventTemUser = new ETeamUser();
            var EventProject = new EProject();
            var EventTeamProject = new ETeamProject();
            var EventWebRequestItem = new EWebRequestItem();
            var EventWebRequestDir = new EWebRequestDir();
            var EventWebRequestParamsItem = new EWebRequestParamsItem();
            var EventWebRequestHistory = new EWebRequestHistory();
            var EventChat = new EChat();
            var EventUserAccess = new EUserAccess();
            var EventUsers = new EUsers();
            var EventTramAccess = new ETeamAccess();
        }
    }
}
