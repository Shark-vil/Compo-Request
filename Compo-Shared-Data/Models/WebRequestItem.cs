using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Compo_Shared_Data.Models
{
    [Serializable]
    public class WebRequestItem
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Link { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        [System.Xml.Serialization.XmlIgnore]
        public Project Projects { get; set; }
    }
}
