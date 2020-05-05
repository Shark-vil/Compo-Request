using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Compo_Shared_Data.Models
{
    [Serializable]
    public class TeamGroup
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string TeamUid { get; set; }
        [Required]
        [ForeignKey("OwnerId")]
        public User Users { get; set; }

        public ICollection<TeamUser> TeamUsers { get; set; }
    }
}
