using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DropDown.Models
{
    [Index("ProjetId", Name = "IX_ActionProjs_ProjetId")]
    public partial class ActionProj
    {
        public ActionProj()
        {
            Objectifs = new HashSet<Objectif>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public int ProjetId { get; set; }

        [ForeignKey("ProjetId")]
        [InverseProperty("ActionProjs")]
        public virtual Projet? Projet { get; set; } = null!;
        [InverseProperty("ActionProj")]
        public virtual ICollection<Objectif> Objectifs { get; set; }
    }
}
