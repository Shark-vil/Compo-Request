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
                    var TeamData = Package.Unpacking<TeamGroup>(ClientResponse.DataBytes);
                    var TeamGroup = db.TeamGroups.Where(t => t.TeamUid == TeamData.TeamUid).FirstOrDefault();

                    if (TeamGroup == null)
                    {
                        var NetworkUser = Users.ActiveUsers.Find(x => x.Id == NetworkClient.Id);
                        TeamData.Users = db.Users.Where(u => u.Login == NetworkUser.Login).FirstOrDefault();

                        db.TeamGroups.Add(TeamData);
                        db.SaveChanges();

                        Debug.Log("В базу данных добавлена новая команда", ConsoleColor.Magenta);

                        var TeamGroupDb = db.TeamGroups.Where(t => t.TeamUid == TeamData.TeamUid).FirstOrDefault();

                        var TeamGroupRequest = new WpfTeamGroup
                        {
                            Id = TeamGroupDb.Id,
                            Title = TeamGroupDb.Title,
                            TeamUid = TeamGroupDb.TeamUid
                        };

                        Sender.Broadcast("TeamGroup.Add.Confirm", TeamGroupRequest);
                        return;
                    }

                    Sender.Send(NetworkClient, "TeamGroup.Add.Error");
                    return;
                }
            }
            catch (DbUpdateException ex)
            {
                Debug.LogError("Возникла ошибка при авторизации пользователя в системе! Код ошибки:\n" + ex);

                Sender.Send(NetworkClient, "TeamGroup.Add.Error");
            }
        }

        private void UpdateTeamGroup(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            using (var db = new DatabaseContext())
            {
                var TGroup = Package.Unpacking<WpfTeamGroup>(ClientResponse.DataBytes);

                TeamGroup DbTeamGroup = db.TeamGroups.Where(x => x.Id == TGroup.Id).FirstOrDefault();
                DbTeamGroup.TeamUid = TGroup.TeamUid;
                DbTeamGroup.Title = TGroup.Title;
                db.SaveChanges();

                Sender.Send(NetworkClient, "TeamGroup.Update.Confirm", default, ClientResponse.WindowUid);
            }
        }

        private void DeleteTeamGroup(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            using (var db = new DatabaseContext())
            {
                var TGroup = Package.Unpacking<WpfTeamGroup>(ClientResponse.DataBytes);

                TeamGroup DbTeamGroup = db.TeamGroups.Where(x => x.Id == TGroup.Id).FirstOrDefault();
                db.TeamGroups.Remove(DbTeamGroup);
                db.SaveChanges();

                Sender.Send(NetworkClient, "TeamGroup.Delete.Confirm", TGroup, ClientResponse.WindowUid);
            }
        }

        private void GetAllTeamGroups(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            using (var db = new DatabaseContext())
            {
                var TeamGroupsDb = db.TeamGroups.ToArray();
                var TeamGroupRequest = new List<WpfTeamGroup>();

                foreach (var TGroup in TeamGroupsDb)
                    TeamGroupRequest.Add(new WpfTeamGroup
                    {
                        Id = TGroup.Id,
                        Title = TGroup.Title,
                        TeamUid = TGroup.TeamUid
                    });

                Sender.Send(NetworkClient, "TeamGroup.GetAll", TeamGroupRequest.ToArray(), ClientResponse.WindowUid);
            }
        }
    }
}
