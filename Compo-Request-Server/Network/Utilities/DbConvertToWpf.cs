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
                TeamUid = DbTeamGroup.TeamUid
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
            WUser[] Users = new WUser[DbUsers.Length];

            for (int i = 0; i < DbUsers.Length; i++)
            {
                Users[i] = ConvertUser(DbUsers[i]);
            }

            WTeamUser[] TeamUsers = new WTeamUser[DbTeamUsers.Length];

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
    }
}
