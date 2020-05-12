using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Compo_Shared_Data.Models
{
    [Serializable]
    public class WebRequestParamsItem
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int WebRequestItemId { get; set; }
        [ForeignKey("WebRequestItemId")]
        [System.Xml.Serialization.XmlIgnore]
        public WebRequestItem WebRequestItems { get; set; }
    }
}
