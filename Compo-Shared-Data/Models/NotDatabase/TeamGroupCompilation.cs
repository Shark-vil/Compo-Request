using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.Models.NotDatabase
{
    [Serializable]
    public class TeamGroupCompilation
    {
        public TeamProject[] TeamProjects;
        public TeamGroup[] TeamGroups;
    }
}
