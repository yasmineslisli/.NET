using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DropDown.Models
{
    [Index("ActionProjId", Name = "IX_Objectifs_ActionProjId")]
    [Index("Drid", Name = "IX_Objectifs_DRId")]
    [Index("ExerciceId", Name = "IX_Objectifs_ExerciceId")]
    public partial class Objectif
    {
        public Objectif()
        {
            Prévisions = new HashSet<Prévision>();
            Réalisations = new HashSet<Réalisation>();
            Stocks = new HashSet<Stock>();
        }

        [Key]
        public int Id { get; set; }
        public int ActionProjId { get; set; }
        public int ExerciceId { get; set; }
        [Column("DRId")]
        public int Drid { get; set; }

        [ForeignKey("ActionProjId")]
        [InverseProperty("Objectifs")]
        public virtual ActionProj? ActionProj { get; set; } = null!;
        [ForeignKey("Drid")]
        [InverseProperty("Objectifs")]
        public virtual Dr? Dr { get; set; } = null!;
        [ForeignKey("ExerciceId")]
        [InverseProperty("Objectifs")]
        public virtual Exercice? Exercice { get; set; } = null!;
        [InverseProperty("Objectif")]
        public virtual ICollection<Prévision> Prévisions { get; set; }
        [InverseProperty("Objectif")]
        public virtual ICollection<Réalisation> Réalisations { get; set; }
        [InverseProperty("Objectif")]
        public virtual ICollection<Stock> Stocks { get; set; }
    }
}
