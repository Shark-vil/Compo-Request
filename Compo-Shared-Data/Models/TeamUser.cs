﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Compo_Shared_Data.Models
{
    [Serializable]
    public class TeamUser
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public int TeamGroupId { get; set; }
        [ForeignKey("TeamGroupId")]
        [System.Xml.Serialization.XmlIgnore]
        public TeamGroup TeamGroups { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        [System.Xml.Serialization.XmlIgnore]
        public User User { get; set; }
    }
}
