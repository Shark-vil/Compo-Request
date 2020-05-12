using System;
using System.Collections.Generic;
using System.Text;

namespace Compo_Shared_Data.Network.Models
{
    [Serializable]
    public class MUserNetwork
    {
        public int Id;
        public string NetworkId;
        public string Name;
        public string Surname;
        public string Patronymic;
        public string Email;
        public string Login;
    }
}
