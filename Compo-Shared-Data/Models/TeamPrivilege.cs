﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Compo_Shared_Data.Models
{
    [Serializable]
    public class TeamPrivilege
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Privilege { get; set; }
        [Required]
        public int TeamGroupId { get; set; }
        [ForeignKey("TeamGroupId")]
        [System.Xml.Serialization.XmlIgnore]
        public TeamGroup TeamGroups { get; set; }
    }
}
