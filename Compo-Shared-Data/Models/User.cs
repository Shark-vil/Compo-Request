using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.Models
{
    [Serializable]
    public class User
    {
        public int Id;
        public string Name;
        public string Surname;
        public string Patronymic;
        public string Email;
        public string Login;
        public string Password;
    }
}
