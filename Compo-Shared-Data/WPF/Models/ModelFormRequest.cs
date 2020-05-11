using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.WPF.Models
{
    [Serializable]
    public class ModelFormRequest
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
    }
}
