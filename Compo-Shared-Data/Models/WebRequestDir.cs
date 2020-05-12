using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Compo_Shared_Data.Models
{
    [Serializable]
    public class WebRequestDir
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public int WebRequestItemId { get; set; }
        [ForeignKey("WebRequestItemId")]
        [System.Xml.Serialization.XmlIgnore]
        public WebRequestItem WebRequestItems { get; set; }
    }
}
