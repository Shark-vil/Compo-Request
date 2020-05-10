using Compo_Request_Server.Network.Server;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.WPF.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request_Server.Network.Utilities
{
    public class DbConvertToWpf
    {
        public static WTeamGroup ConvertTeamGroup(TeamGroup DbTeamGroup)
        {
            return new WTeamGroup
            {
                Id = DbTeamGroup.Id,
                Title = DbTeamGroup.Title,
                Uid = DbTeamGroup.Uid
            };
        }

        public static WTeamGroup[] ConvertTeamGroup(TeamGroup[] DbTeamGroups)
        {
            WTeamGroup[] WTeamGroups = new WTeamGroup[DbTeamGroups.Length];

            for (int i = 0; i < DbTeamGroups.Length; i++)
            {
                WTeamGroups[i] = ConvertTeamGroup(DbTeamGroups[i]);
            }

            return WTeamGroups;
        }

        public static WTeamProject ConvertTeamProject(TeamProject DbTeamProject)
        {
            return new WTeamProject
            {
                Id = DbTeamProject.Id,
                ProjectId = DbTeamProject.ProjectId,
                TeamGroupId = DbTeamProject.TeamGroupId
            };
        }

        public static WUser ConvertUser(User DbUser)
        {
            return new WUser
            {
                Id = DbUser.Id,
                Login = DbUser.Login,
                Email = DbUser.Email,
                Name = DbUser.Name,
                Surname = DbUser.Surname,
                Patronymic = DbUser.Patronymic
            };
        }

        public static WTeamUser ConvertTeamUser(TeamUser DbTeamUser)
        {
            return new WTeamUser
            {
                Id = DbTeamUser.Id,
                TeamGroupId = DbTeamUser.TeamGroupId,
                UserId = DbTeamUser.UserId
            };
        }

        public static WTeamUserCompilation ConvertTeamUserCompilation(User[] DbUsers, TeamUser[] DbTeamUsers)
        {
            var Users = new WUser[DbUsers.Length];

            for (int i = 0; i < DbUsers.Length; i++)
            {
                Users[i] = ConvertUser(DbUsers[i]);
            }

            var TeamUsers = new WTeamUser[DbTeamUsers.Length];

            for (int i = 0; i < DbTeamUsers.Length; i++)
            {
                TeamUsers[i] = ConvertTeamUser(DbTeamUsers[i]);
            }

            return new WTeamUserCompilation
            {
                TeamUsers = TeamUsers,
                Users = Users
            };
        }

        public static WTeamGroupCompilation ConvertTeamProjectCompilation(TeamGroup[] DbTeamGroup, TeamProject[] DbTeamProject)
        {
           var TeamGroups = new WTeamGroup[DbTeamGroup.Length];

            for (int i = 0; i < DbTeamGroup.Length; i++)
            {
                TeamGroups[i] = ConvertTeamGroup(DbTeamGroup[i]);
            }

            var TeamProjects = new WTeamProject[DbTeamProject.Length];

            for (int i = 0; i < DbTeamProject.Length; i++)
            {
                TeamProjects[i] = ConvertTeamProject(DbTeamProject[i]);
            }

            return new WTeamGroupCompilation
            {
                WTeamGroups = TeamGroups,
                WTeamProjects = TeamProjects
            };
        }
    }
}
