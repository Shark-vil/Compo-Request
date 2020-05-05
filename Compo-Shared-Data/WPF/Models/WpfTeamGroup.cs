using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.WPF.Models
{
    [Serializable]
    public class WpfTeamGroup
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string TeamUid { get; set; }
    }
}
