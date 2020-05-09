using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Compo_Shared_Data.Models
{
    [Serializable]
    public class User
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public ICollection<TeamUser> TeamUsers { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        public ICollection<TeamGroup> TeamGroups { get; set; }
        [System.Xml.Serialization.XmlIgnore]
        public ICollection<Project> Projects { get; set; }
    }
}
