using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Compo_Shared_Data.Models
{
    [Serializable]
    public class WebRequestHistory
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        public string Link { get; set; }
        [Required]
        public string Method { get; set; }

        [Required]
        public string ParametrsInJson { get; set; }

        [Required]
        public DateTime ResponseDate { get; set; }
        [Required]
        public string ResponseResult { get; set; }

        [Required]
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        [System.Xml.Serialization.XmlIgnore]
        public Project Projects { get; set; }
    }
}
