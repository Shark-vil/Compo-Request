using Compo_Request_Server.Network.Database;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Request_Server.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Compo_Shared_Data.Models;
using System.Linq;
using Compo_Shared_Data.WPF.Models;

namespace Compo_Request_Server.Network.Events.Team
{
    public class ETeam
    {
        public ETeam()
        {
            NetworkDelegates.Add(AddTeamGroup, "TeamGroup.Add");
            NetworkDelegates.Add(GetAllTeamGroups, "TeamGroup.GetAll");
            NetworkDelegates.Add(UpdateTeamGroup, "TeamGroup.Update");
            NetworkDelegates.Add(DeleteTeamGroup, "TeamGroup.Delete");
        }

        private void AddTeamGroup(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            if (!AccessController.IsPrivilege(NetworkClient, "teams.add"))
                return;

            try
            {
                using (var db = new DatabaseContext())
                {
                    var MTeamGroup = Package.Unpacking<TeamGroup>(ClientResponse.DataBytes);
                    var DbTeamGroup = db.TeamGroups.Where(t => t.Uid == MTeamGroup.Uid).FirstOrDefault();

                    if (DbTeamGroup == null)
                    {
                        var NetworkUser = Users.ActiveUsers.Find(x => x.NetworkId == NetworkClient.Id);

                        MTeamGroup.User = db.Users.FirstOrDefault(u => u.Login == NetworkUser.Login);

                        db.TeamGroups.Add(MTeamGroup);
                        db.SaveChanges();

                        Debug.Log($"В базу данных добавлена новая команда:\n" +
                            $"Id - {MTeamGroup.Id}\n" +
                            $"Uid - {MTeamGroup.Uid}\n" +
                            $"Title - {MTeamGroup.Title}\n" + 
                            $"UserId - {MTeamGroup.UserId}", ConsoleColor.Magenta);

                        Sender.Broadcast("TeamGroup.Add.Confirm", MTeamGroup);
                        return;
                    }

                    Sender.Send(NetworkClient, "TeamGroup.Add.Error");
                    return;
                }
            }
            catch (DbUpdateException ex)
            {
                Debug.LogError("Возникла ошибка при добавлении команды в базу данных! Код ошибки:\n" + ex);

                Sender.Send(NetworkClient, "TeamGroup.Add.Error");
            }
        }

        private void UpdateTeamGroup(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            if (!AccessController.IsPrivilege(NetworkClient, "teams.edit"))
                return;

            try
            {
                using (var db = new DatabaseContext())
                {
                    var MTeamGroup = Package.Unpacking<TeamGroup>(ClientResponse.DataBytes);

                    var DbTeamGroupCache = new TeamGroup
                    {
                        Id = MTeamGroup.Id,
                        Uid = MTeamGroup.Uid,
                        Title = MTeamGroup.Title,
                        UserId = MTeamGroup.UserId
                    };

                    var DbTeamGroup = db.TeamGroups.FirstOrDefault(t => t.Id == MTeamGroup.Id);

                    DbTeamGroup.Title = MTeamGroup.Title;
                    DbTeamGroup.Uid = MTeamGroup.Uid;

                    db.SaveChanges();

                    Debug.Log($"Информация о команде обновлена:\n" +
                            $"Id - {DbTeamGroupCache.Id} > {DbTeamGroup.Id}\n" +
                            $"TeamUid - {DbTeamGroupCache.Uid} > {DbTeamGroup.Uid}\n" +
                            $"Title - {DbTeamGroupCache.Title} > {DbTeamGroup.Title}\n" +
                            $"UserId - {DbTeamGroupCache.UserId} > {DbTeamGroup.UserId}", ConsoleColor.Magenta);

                    Sender.Broadcast("TeamGroup.Update.Confirm", DbTeamGroup, ClientResponse.WindowUid);
                }
            }
            catch(DbUpdateException ex)
            {
                Debug.LogError("Возникла ошибка при обновлении команды в базе данных! Код ошибки:\n" + ex);

                Sender.Send(NetworkClient, "TeamGroup.Update.Error");
            }
        }

        private void DeleteTeamGroup(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            if (!AccessController.IsPrivilege(NetworkClient, "teams.delete"))
                return;

            try
            {
                using (var db = new DatabaseContext())
                {
                    var MTeamGroup = Package.Unpacking<TeamGroup>(ClientResponse.DataBytes);

                    foreach (var TeamUser in db.TeamUsers.Where(tu => tu.TeamGroupId == MTeamGroup.Id).ToArray())
                        db.TeamUsers.Remove(TeamUser);

                    db.TeamGroups.Attach(MTeamGroup);
                    db.TeamGroups.Remove(MTeamGroup);
                    db.SaveChanges();

                    Debug.Log($"Команда удалена:\n" +
                            $"Id - {MTeamGroup.Id}\n" +
                            $"TeamUid - {MTeamGroup.Uid}\n" +
                            $"Title - {MTeamGroup.Title}\n" +
                            $"UserId - {MTeamGroup.UserId}", ConsoleColor.Magenta);

                    Sender.Broadcast("TeamGroup.Delete.Confirm", MTeamGroup, ClientResponse.WindowUid);
                }
            }
            catch (DbUpdateException ex)
            {
                Debug.LogError("Возникла ошибка при удалении команды из базы данных! Код ошибки:\n" + ex);

                Sender.Send(NetworkClient, "TeamGroup.Delete.Error");
            }
        }

        private void GetAllTeamGroups(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            if (!AccessController.IsPrivilege(NetworkClient, "teams"))
                return;

            try
            {
                using (var db = new DatabaseContext())
                {
                    var TeamGroupsDb = db.TeamGroups.ToArray();

                    Debug.Log($"Получен список команд из базы данных в количестве {TeamGroupsDb.Length} записей.", ConsoleColor.Magenta);

                    Sender.Send(NetworkClient, "TeamGroup.GetAll", TeamGroupsDb, ClientResponse.WindowUid);
                }
            }
            catch(Exception ex)
            {
                Debug.LogError("Возникла ошибка при получении списка команд из базы данных! Код ошибки:\n" + ex);

                Sender.Send(NetworkClient, "TeamGroup.GetAll.Error");
            }
        }
    }
}
