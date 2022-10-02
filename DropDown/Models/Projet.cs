using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DropDown.Models
{
    [Index("ProgrammeId", Name = "IX_Projets_ProgrammeId")]
    public partial class Projet
    {
        public Projet()
        {
            ActionProjs = new HashSet<ActionProj>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int ProgrammeId { get; set; }

        [ForeignKey("ProgrammeId")]
        [InverseProperty("Projets")]
        public virtual Programme? Programme { get; set; } = null!;
        [InverseProperty("Projet")]
        public virtual ICollection<ActionProj> ActionProjs { get; set; }
    }
}
