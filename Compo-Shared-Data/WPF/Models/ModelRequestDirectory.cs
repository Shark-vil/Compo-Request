using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.WPF.Models
{
    [Serializable]
    public class ModelRequestDirectory
    {
        public int Id { get; set; }
        public int WebRequestId { get; set; }
        public string RequestTitle { get; set; }
        public string Title { get; set; }
        public string WebRequest { get; set; }
        public string RequestMethod { get; set; }
    }
}
