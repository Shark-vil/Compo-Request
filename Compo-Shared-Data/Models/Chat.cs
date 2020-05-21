using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Compo_Shared_Data.Models
{
    [Serializable]
    public class Chat
    {
        [Key]
        [Required]
        public int Id { get; set; }

        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        [System.Xml.Serialization.XmlIgnore]
        public Project Projects { get; set; }

        [Required]
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        [System.Xml.Serialization.XmlIgnore]
        public User User { get; set; }
    }
}
