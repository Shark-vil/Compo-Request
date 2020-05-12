using Compo_Request.Windows.Editor;
using Compo_Shared_Data.Models;
using Compo_Shared_Data.WPF.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Request.Models
{
    public static class ProjectData
    {
        public static Project SelectedProject { get; set; }
        public static BoundModel TabCollecton { get; set; }
        public static ModelRequestDirectory RequestDirectory { get; set; }
    }
}
