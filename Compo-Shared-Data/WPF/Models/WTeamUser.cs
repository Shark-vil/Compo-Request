using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.WPF.Models
{
    [Serializable]
    public class WTeamUser
    {
        public int Id;
        public int TeamGroupId;
        public int UserId;
    }

    [Serializable]
    public class WTeamUserCompilation
    {
        public WTeamUser[] TeamUsers;
        public WUser[] Users;
    }
}
