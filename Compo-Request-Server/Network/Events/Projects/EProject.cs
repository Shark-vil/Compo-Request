using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using Compo_Request_Server.Network.Database;
using Compo_Request_Server.Network.Models;
using Compo_Request_Server.Network.Server;
using Compo_Request_Server.Network.Utilities;
using Compo_Shared_Data.Debugging;
using Compo_Shared_Data.Network;
using Compo_Shared_Data.Network.Models;
using Compo_Shared_Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Compo_Request_Server.Network.Events.Projects
{
    public class EProject
    {
        public EProject()
        {
            NetworkDelegates.Add(AddProject, "Project.Add");
            NetworkDelegates.Add(GetAllProjects, "Project.GetAll");
        }

        private void GetAllProjects(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var DbProjects = db.Projects.ToArray();

                    Debug.Log($"Получен список команд из базы данных в количестве {DbProjects.Length} записей.", ConsoleColor.Magenta);

                    Sender.Send(NetworkClient, "Project.GetAll", DbProjects, ClientResponse.WindowUid);
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("Возникла ошибка при получении списка команд из базы данных! Код ошибки:\n" + ex);

                Sender.Send(NetworkClient, "Project.GetAll.Error");
            }
        }

        private void AddProject(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var MPorject = Package.Unpacking<Project>(ClientResponse.DataBytes);
                    MPorject.UserId = Users.GetUserById(NetworkClient.Id).Id;

                    db.Projects.Attach(MPorject);
                    db.SaveChanges();

                    Sender.Broadcast("Project.Add.Confirm", MPorject);
                }
            }
            catch(DbException ex)
            {

            }
        }
    }
}
