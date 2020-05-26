using Compo_Shared_Data.Network.Models;
using Compo_Shared_Data.WPF.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request.Network.Client
{
    public static class UserInfo
    {
        public static MUserNetwork NetworkSelf;
        public static List<MUserNetwork> NetworkUsers = new List<MUserNetwork>();

        public static WUser MUserToWUser(MUserNetwork User, int TeamGroupId = 0)
        {
            return new WUser
            {
                Id = User.Id,
                Login = User.Login,
                Email = User.Email,
                Name = User.Name,
                Surname = User.Surname,
                Patronymic = User.Patronymic,
                TeamGroupId = TeamGroupId
            };
        }
    }
}
