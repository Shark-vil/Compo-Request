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
            try
            {
                using (var db = new DatabaseContext())
                {
                    var TeamData = Package.Unpacking<WTeamGroup>(ClientResponse.DataBytes);
                    var TeamGroup = db.TeamGroups.Where(t => t.TeamUid == TeamData.TeamUid).FirstOrDefault();

                    if (TeamGroup == null)
                    {
                        var NetworkUser = Users.ActiveUsers.Find(x => x.Id == NetworkClient.Id);

                        var DbTeamGroup = new TeamGroup();
                        DbTeamGroup.TeamUid = TeamData.TeamUid;
                        DbTeamGroup.Title = TeamData.Title;
                        DbTeamGroup.User = db.Users.Where(u => u.Login == NetworkUser.Login).FirstOrDefault();

                        db.TeamGroups.Add(DbTeamGroup);
                        db.SaveChanges();

                        Debug.Log($"В базу данных добавлена новая команда:\n" +
                            $"Id - {DbTeamGroup.Id}\n" +
                            $"TeamUid - {DbTeamGroup.TeamUid}\n" +
                            $"Title - {DbTeamGroup.Title}\n" + 
                            $"UserId - {DbTeamGroup.UserId}", ConsoleColor.Magenta);

                        Sender.Broadcast("TeamGroup.Add.Confirm", DbConvertToWpf.ConvertTeamGroup(DbTeamGroup));
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
            try
            {
                using (var db = new DatabaseContext())
                {
                    var TGroup = Package.Unpacking<WTeamGroup>(ClientResponse.DataBytes);

                    TeamGroup DbTeamGroup = WpfConvertToBd.ConvertTeamGroup(db, TGroup);
                    TeamGroup DbTeamGroupCache = new TeamGroup
                    {
                        Id = DbTeamGroup.Id,
                        TeamUid = DbTeamGroup.TeamUid,
                        Title = DbTeamGroup.Title,
                        UserId = DbTeamGroup.UserId
                    };

                    DbTeamGroup.TeamUid = TGroup.TeamUid;
                    DbTeamGroup.Title = TGroup.Title;

                    db.SaveChanges();

                    Debug.Log($"Информация о команде обновлена:\n" +
                            $"Id - {DbTeamGroupCache.Id} > {DbTeamGroup.Id}\n" +
                            $"TeamUid - {DbTeamGroupCache.TeamUid} > {DbTeamGroup.TeamUid}\n" +
                            $"Title - {DbTeamGroupCache.Title} > {DbTeamGroup.Title}\n" +
                            $"UserId - {DbTeamGroupCache.UserId} > {DbTeamGroup.UserId}", ConsoleColor.Magenta);

                    Sender.Broadcast("TeamGroup.Update.Confirm", TGroup, ClientResponse.WindowUid);
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
            try
            {
                using (var db = new DatabaseContext())
                {
                    var TGroup = Package.Unpacking<WTeamGroup>(ClientResponse.DataBytes);

                    TeamGroup DbTeamGroup = WpfConvertToBd.ConvertTeamGroup(db, TGroup);
                    db.TeamGroups.Remove(DbTeamGroup);
                    db.SaveChanges();

                    Debug.Log($"Команда удалена:\n" +
                            $"Id - {DbTeamGroup.Id}\n" +
                            $"TeamUid - {DbTeamGroup.TeamUid}\n" +
                            $"Title - {DbTeamGroup.Title}\n" +
                            $"UserId - {DbTeamGroup.UserId}", ConsoleColor.Magenta);

                    Sender.Broadcast("TeamGroup.Delete.Confirm", TGroup, ClientResponse.WindowUid);
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
            try
            {
                using (var db = new DatabaseContext())
                {
                    var TeamGroupsDb = db.TeamGroups.ToArray();

                    Debug.Log($"Получен список команд из базы данных в количестве {TeamGroupsDb.Length} записей.", ConsoleColor.Magenta);

                    Sender.Send(NetworkClient, "TeamGroup.GetAll",
                        DbConvertToWpf.ConvertTeamGroup(TeamGroupsDb), ClientResponse.WindowUid);
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
