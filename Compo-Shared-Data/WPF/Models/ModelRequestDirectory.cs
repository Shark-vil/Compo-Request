using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.WPF.Models
{
    [Serializable]
    public class ModelRequestDirectory
    {
        public int CollectionIndex { get; set; }
        public string RequestDir { get; set; }
        public string WebRequest { get; set; }
        public string RequestMethod { get; set; }
    }
}
