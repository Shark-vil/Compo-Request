using Compo_Request_Server.Network.Database;
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
    public class ETeamAccess
    {
        public ETeamAccess()
        {
            NetworkDelegates.Add(AccessGetAll, "Team.Access.GetAll");
            NetworkDelegates.Add(AccessUpdate, "Team.Access.Update");
        }

        private void AccessGetAll(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            if (!AccessController.IsPrivilege(NetworkClient, "teams.access"))
                return;

            try
            {
                int TeamId = Package.Unpacking<int>(ClientResponse.DataBytes);

                using (var db = new DatabaseContext())
                {
                    var DbTeamPrivileges = db.TeamPrivileges.Where(x => x.TeamGroupId == TeamId).ToArray();

                    Debug.Log($"Получен список прав доступа в количестве {DbTeamPrivileges.Length} записей.", ConsoleColor.Magenta);

                    Sender.Send(NetworkClient, "Team.Access.GetAll.Confirm",
                        DbTeamPrivileges, ClientResponse.WindowUid);
                }
            }
            catch (DbException ex)
            {
                Debug.LogError("Возникло исключение при получении списка прав доступа. Код ошибки:\n" + ex);
                Sender.Send(NetworkClient, "Team.Access.GetAll.Error");
            }
        }

        private void AccessUpdate(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            if (!AccessController.IsPrivilege(NetworkClient, "teams.access"))
                return;

            try
            {
                using (var db = new DatabaseContext())
                {
                    var ReadTeamPrivileges = Package.Unpacking<MTeamPrivilegeTransport>(ClientResponse.DataBytes);
                    var DbTeamPrivileges = db.TeamPrivileges.Where(x => x.TeamGroupId == ReadTeamPrivileges.TeamGroupId).ToArray();
                    var TeamGroup = db.TeamGroups.FirstOrDefault(x => x.Id == ReadTeamPrivileges.TeamGroupId);

                    Debug.Log($"Обновление прав доступа команды [{TeamGroup.Id}] - {TeamGroup.Uid}", ConsoleColor.Magenta);

                    List<TeamPrivilege> DataRemoves = new List<TeamPrivilege>();
                    List<string> DataExists = new List<string>();

                    Debug.Log($"\nСписок прав на удаление:", ConsoleColor.Magenta);
                    foreach (var UserPrivilege in DbTeamPrivileges)
                        if (!Array.Exists(ReadTeamPrivileges.Privileges, x => x.Privilege == UserPrivilege.Privilege))
                        {
                            DataRemoves.Add(UserPrivilege);
                            Debug.Log($"> Privilege - {UserPrivilege.Privilege}", ConsoleColor.Magenta);
                        }
                        else
                            DataExists.Add(UserPrivilege.Privilege);

                    foreach (var TeamPrivilege in DataRemoves)
                        db.TeamPrivileges.Remove(TeamPrivilege);

                    Debug.Log($"\nСписок прав на добавление:", ConsoleColor.Magenta);
                    foreach (var TeamPrivilege in ReadTeamPrivileges.Privileges)
                    {
                        if (!DataExists.Exists(x => x == TeamPrivilege.Privilege))
                        {
                            db.TeamPrivileges.Add(TeamPrivilege);
                            Debug.Log($"> Privilege - {TeamPrivilege.Privilege} : TeamGroupId - {TeamPrivilege.TeamGroupId}",
                                ConsoleColor.Magenta);
                        }
                    }

                    db.SaveChanges();

                    Debug.Log($"\nПрава команды [{TeamGroup.Id}] - {TeamGroup.Uid} обновлены!\n", ConsoleColor.Magenta);

                    DbTeamPrivileges = db.TeamPrivileges.Where(x => x.TeamGroupId == ReadTeamPrivileges.TeamGroupId).ToArray();

                    Sender.Broadcast("Team.Access.Update.Confirm",
                        DbTeamPrivileges, ClientResponse.WindowUid);
                }
            }
            catch (DbException ex)
            {
                Debug.LogError("Возникло исключение при обновлении списка прав доступа. Код ошибки:\n" + ex);
                Sender.Send(NetworkClient, "Team.Access.Update.Error");
            }
        }
    }
}
