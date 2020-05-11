using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.WPF.Models
{
    public class ModelEditorRequest
    {
        public List<string> RequestTypes = new List<string>
        {
            "POST",
            "GET",
            "PUT",
            "DELETE",
            "UPDATE"
        };
        public string RequestLink { get; set; }
    }
}
