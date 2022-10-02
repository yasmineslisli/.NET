using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DropDown.Models
{
    [Index("ObjectifId", Name = "IX_Réalisations_objectifId")]
    public partial class Réalisation
    {
        [Key]
        public int Id { get; set; }
        
        public int Nombre { get; set; }
        public int Superficie { get; set; }
        public int Valeur { get; set; }
        [Column("date")]
        public DateTime Date { get; set; }
        [Column("objectifId")]
        public int ObjectifId { get; set; }

        [ForeignKey("ObjectifId")]
        [InverseProperty("Réalisations")]
        public virtual Objectif? Objectif { get; set; } = null!;
    }
}
