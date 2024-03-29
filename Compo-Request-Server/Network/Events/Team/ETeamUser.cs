﻿using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using Compo_Request_Server.Network.Database;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Request_Server.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using Compo_Shared_Data.WPF.Models;
using Microsoft.EntityFrameworkCore;

namespace Compo_Request_Server.Network.Events.Team
{
    public class ETeamUser
    {
        public ETeamUser()
        {
            NetworkDelegates.Add(GetTeamUsers, "TeamUser.Get");
            NetworkDelegates.Add(SaveTeamUser, "TeamUser.Save");
        }

        private void SaveTeamUser(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            if (!AccessController.IsPrivilege(NetworkClient, "teams.edit"))
                return;

            try
            {
                using (var db = new DatabaseContext())
                {
                    var TeamUserTeamId = Package.Unpacking<WTeamUserTeamId>(ClientResponse.DataBytes);

                    foreach (var User in TeamUserTeamId.Users)
                    {
                        TeamUser DbTeamUser = new TeamUser
                        {
                            UserId = User.Id,
                            TeamGroupId = TeamUserTeamId.TeamGroupId
                        };

                        if (db.TeamUsers.FirstOrDefault(tu =>
                            tu.TeamGroupId == DbTeamUser.TeamGroupId && tu.UserId == DbTeamUser.UserId) != null)
                        {
                            continue;
                        }

                        Debug.Log("DbTeamUser:\n" +
                            "UserId - " + DbTeamUser.UserId + "\n" +
                            "TemGroupId - " + DbTeamUser.TeamGroupId);

                        db.TeamUsers.Attach(DbTeamUser);
                        db.SaveChanges();
                    }

                    var DbTeamUsers = db.TeamUsers.Where(tu =>
                        tu.TeamGroupId == TeamUserTeamId.TeamGroupId).ToArray();

                    List<TeamUser> RemoveTeamUsers = new List<TeamUser>();
                    foreach (var DbTeamUser in DbTeamUsers)
                    {
                        bool IsContinue = false;
                        foreach (var User in TeamUserTeamId.Users)
                        {
                            /*
                            if (User.TeamGroupId != DbTeamUser.TeamGroupId)
                            {
                                IsContinue = true;
                                break;
                            }
                            */

                            if (User.Id == DbTeamUser.UserId)
                            {
                                IsContinue = true;
                                break;
                            }
                        }

                        if (IsContinue)
                            continue;

                        RemoveTeamUsers.Add(DbTeamUser);
                    }

                    foreach (var DbTeamUser in RemoveTeamUsers)
                    {
                        db.TeamUsers.Remove(DbTeamUser);
                        db.SaveChanges();
                    }

                    Sender.Broadcast("TeamUser.Save.Confirm", 
                        db.TeamUsers.Where(tu => tu.TeamGroupId == TeamUserTeamId.TeamGroupId).ToArray());
                }
            }
            catch(DbUpdateException ex)
            {
                Debug.LogError("Возникла ошибка при сохранении списка пользователей и команд в базе данных! Код ошибки:\n" + ex);

                Sender.Send(NetworkClient, "TeamUser.Save.Error");
            }
        }

        private void GetTeamUsers(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            if (!AccessController.IsPrivilege(NetworkClient, "teams"))
                return;

            try
            {
                using (var db = new DatabaseContext())
                {
                    var UsersDb = db.Users.ToArray();

                    Debug.Log($"Получен список пользователей из базы данных в количестве {UsersDb.Length} записей.", ConsoleColor.Magenta);

                    var TGroup = Package.Unpacking<TeamGroup>(ClientResponse.DataBytes);
                    var TeamUsersDb = db.TeamUsers.Where(t => t.TeamGroupId == TGroup.Id).ToArray();

                    Debug.Log($"Получен список команд и пользователей из базы данных в количестве {TeamUsersDb.Length} записей.", ConsoleColor.Magenta);

                    Sender.Send(NetworkClient, "TeamUser.Get",
                        DbConvertToWpf.ConvertTeamUserCompilation(UsersDb, TeamUsersDb), ClientResponse.WindowUid);
                }
            }
            catch (DbException ex)
            {
                Debug.LogError("Возникла ошибка при получении списка пользователей из базы данных! Код ошибки:\n" + ex);

                Sender.Send(NetworkClient, "TeamUser.Get.Error");
            }
        }
    }
}
