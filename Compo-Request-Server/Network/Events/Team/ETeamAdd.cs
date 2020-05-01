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

namespace Compo_Request_Server.Network.Events.Team
{
    public class ETeamAdd
    {
        public ETeamAdd()
        {
            NetworkDelegates.Add(AddTeam, "Team.Add");
        }

        private void AddTeam(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var TeamData = Package.Unpacking<TeamGroup>(ClientResponse.DataBytes);
                    var TeamGroup = db.TeamGroups.Where(t => t.TeamUid == TeamData.TeamUid).FirstOrDefault();

                    if (TeamGroup == null)
                    {
                        db.TeamGroups.Add(TeamData);
                        db.SaveChanges();

                        Debug.Log("В базу данных добавлена новая команда", ConsoleColor.Magenta);
                        Sender.Send(NetworkClient, "Team.Add.Confirm");
                    }

                    Sender.Send(NetworkClient, "Team.Add.Error");
                    return;
                }
            }
            catch (DbUpdateException ex)
            {
                Debug.LogError("Возникла ошибка при авторизации пользователя в системе! Код ошибки:\n" + ex);

                Sender.Send(NetworkClient, "Team.Add.Error");
            }
        }
    }
}
