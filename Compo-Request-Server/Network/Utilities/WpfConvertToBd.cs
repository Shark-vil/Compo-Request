using Compo_Request_Server.Network.Database;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.WPF.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Compo_Request_Server.Network.Utilities
{
    public class WpfConvertToBd
    {
        public static TeamGroup ConvertTeamGroup(DatabaseContext db, WTeamGroup WpfTeamGroup)
        {
            return db.TeamGroups.Where(t => t.Id == WpfTeamGroup.Id).FirstOrDefault();
        }
    }
}
