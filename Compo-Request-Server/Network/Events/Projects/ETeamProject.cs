using Compo_Request_Server.Network.Database;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Request_Server.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.Models.NotDatabase;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using Compo_Shared_Data.WPF.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Compo_Request_Server.Network.Events.Projects
{
    public class ETeamProject
    {
        public ETeamProject()
        {
            NetworkDelegates.Add(GetTeamProjects, "TeamProject.Get");
            NetworkDelegates.Add(SaveTeamProjects, "TeamProject.Save");
        }

        private void SaveTeamProjects(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var TeamGroupProjectId = Package.Unpacking<WTeamGroupProjectId>(ClientResponse.DataBytes);

                    foreach (var TeamGroup in TeamGroupProjectId.TeamGroups)
                    {
                        TeamProject DbTeamProject = new TeamProject
                        {
                            ProjectId = TeamGroup.ProjectId,
                            TeamGroupId = TeamGroup.Id
                        };

                        if (db.TeamProjects.Where(tp =>
                            tp.TeamGroupId == DbTeamProject.TeamGroupId && tp.ProjectId == DbTeamProject.ProjectId)
                            .FirstOrDefault() != null)
                        {
                            continue;
                        }

                        db.TeamProjects.Attach(DbTeamProject);
                        db.SaveChanges();
                    }

                    var DbTeamProjects = db.TeamProjects.Where(tp =>
                        tp.TeamGroupId == TeamGroupProjectId.ProjectId).ToArray();

                    List<TeamProject> RemoveTeamProjects = new List<TeamProject>();
                    foreach (var DbTeamProject in DbTeamProjects)
                    {
                        bool IsContinue = false;
                        foreach (var TeamGroup in TeamGroupProjectId.TeamGroups)
                        {
                            if (TeamGroup.Id == DbTeamProject.TeamGroupId)
                            {
                                IsContinue = true;
                                break;
                            }
                        }

                        if (IsContinue)
                            continue;

                        RemoveTeamProjects.Add(DbTeamProject);
                    }

                    foreach (var DbTeamProject in RemoveTeamProjects)
                    {
                        db.TeamProjects.Remove(DbTeamProject);
                        db.SaveChanges();
                    }

                    Sender.Broadcast("TeamProject.Save.Confirm",
                        db.TeamProjects.Where(tu => tu.ProjectId == TeamGroupProjectId.ProjectId).ToArray());
                }
            }
            catch (DbUpdateException ex)
            {
                Debug.LogError("Возникла ошибка при сохранении списка проектов и команд в базе данных! Код ошибки:\n" + ex);

                Sender.Send(NetworkClient, "TeamProject.Save.Error");
            }
        }

        private void GetTeamProjects(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var TeamGroups = db.TeamGroups.ToArray();

                    Debug.Log($"Получен список команд из базы данных в количестве {TeamGroups.Length} записей.", ConsoleColor.Magenta);

                    var MProject = Package.Unpacking<Project>(ClientResponse.DataBytes);
                    var DbProjects = db.TeamProjects.Where(t => t.ProjectId == MProject.Id).ToArray();

                    Debug.Log($"Получен список проектов и команд из базы данных в количестве {DbProjects.Length} записей.", ConsoleColor.Magenta);

                    Sender.Send(NetworkClient, "TeamProject.Get", 
                        DbConvertToWpf.ConvertTeamProjectCompilation(TeamGroups, DbProjects), ClientResponse.WindowUid);
                }
            }
            catch (DbException ex)
            {
                Debug.LogError("Возникла ошибка при получении списка проектов и команд из базы данных! Код ошибки:\n" + ex);

                Sender.Send(NetworkClient, "TeamProject.Get.Error");
            }
        }
    }
}
