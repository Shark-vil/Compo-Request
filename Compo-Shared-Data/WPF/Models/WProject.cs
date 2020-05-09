using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.WPF.Models
{
    [Serializable]
    public class WProject
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Uid { get; set; }
        public int UserId { get; set; }
    }
}
