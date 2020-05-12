using Compo_Shared_Data.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.WPF.Models
{
    [Serializable]
    public class WTeamGroup
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Uid { get; set; }
        public int UserId { get; set; }
        // WPF DataGrid CheckBox
        public bool IsSelected { get; set; }
        // WPF ProjectCache
        public int ProjectId { get; set; }
    }

    [Serializable]
    public class WTeamGroupCompilation
    {
        public WTeamProject[] WTeamProjects;
        public WTeamGroup[] WTeamGroups;
    }

    [Serializable]
    public class WTeamGroupProjectId
    {
        public int ProjectId;
        public WTeamGroup[] TeamGroups;
    }
}
