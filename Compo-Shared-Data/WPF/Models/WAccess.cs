using Compo_Shared_Data.Network.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.WPF.Models
{
    [Serializable]
    public class WAccess
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public bool IsSelected { get; set; }
    }
}
