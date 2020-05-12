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
            NetworkDelegates.Add(UpdateProject, "Project.Update");
            NetworkDelegates.Add(DeleteProject, "Project.Delete");
        }

        private void DeleteProject(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var MProject = Package.Unpacking<Project>(ClientResponse.DataBytes);

                    db.Projects.Attach(MProject);
                    db.Projects.Remove(MProject);
                    db.SaveChanges();

                    Debug.Log($"Команда удалена:\n" +
                            $"Id - {MProject.Id}\n" +
                            $"TeamUid - {MProject.Uid}\n" +
                            $"Title - {MProject.Title}\n" +
                            $"UserId - {MProject.UserId}", ConsoleColor.Magenta);

                    Sender.Broadcast("Project.Delete.Confirm", MProject, ClientResponse.WindowUid);
                }
            }
            catch (DbUpdateException ex)
            {
                Debug.LogError("Возникла ошибка при удалении команды из базы данных! Код ошибки:\n" + ex);

                Sender.Send(NetworkClient, "Project.Delete.Error");
            }
        }

        private void UpdateProject(MResponse ClientResponse, MNetworkClient NetworkClient)
        {
            try
            {
                using (var db = new DatabaseContext())
                {
                    var MProject = Package.Unpacking<Project>(ClientResponse.DataBytes);

                    Project DbProject = db.Projects.Where(p => p.Id == MProject.Id).FirstOrDefault();

                    Project DbProjectCache = new Project
                    {
                        Id = DbProject.Id,
                        Uid = DbProject.Uid,
                        Title = DbProject.Title,
                        UserId = DbProject.UserId
                    };

                    DbProject.Uid = MProject.Uid;
                    DbProject.Title = MProject.Title;

                    db.SaveChanges();

                    Debug.Log($"Информация о команде обновлена:\n" +
                            $"Id - {DbProjectCache.Id} > {MProject.Id}\n" +
                            $"TeamUid - {DbProjectCache.Uid} > {MProject.Uid}\n" +
                            $"Title - {DbProjectCache.Title} > {MProject.Title}\n" +
                            $"UserId - {DbProjectCache.UserId} > {MProject.UserId}", ConsoleColor.Magenta);

                    Sender.Broadcast("Project.Update.Confirm", DbProject, ClientResponse.WindowUid);
                }
            }
            catch (DbUpdateException ex)
            {
                Debug.LogError("Возникла ошибка при обновлении команды в базе данных! Код ошибки:\n" + ex);

                Sender.Send(NetworkClient, "Project.Update.Error");
            }
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
