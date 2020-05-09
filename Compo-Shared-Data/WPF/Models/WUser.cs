using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.WPF.Models
{
    [Serializable]
    public class WUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string Email { get; set; }
        public string Login { get; set; }

        // WPF DataGrid CheckBox
        public bool IsSelected { get; set; }
        // WPF TeamCache
        public int TeamGroupId { get; set; }
    }
}
