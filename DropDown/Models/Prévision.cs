using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DropDown.Models
{
    [Index("ObjectifId", Name = "IX_Prévisions_objectifId")]
    public partial class Prévision
    {
        public Prévision()
        {
            Details = new HashSet<Detail>();
        }

        [Key]
        public int Id { get; set; }
        

        [Range(1, int.MaxValue, ErrorMessage = "Veuillez entrer une valeur supérieure à 0")]
        public int Nombre { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Veuillez entrer une valeur supérieure à 0")]
        public int Superficie { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Veuillez entrer une valeur supérieure à 0")]
        public int? Valeur { get; set; }
        [Column("date")]
        public string Date { get; set; }
        [Column("objectifId")]
        public int ObjectifId { get; set; }

        [ForeignKey("ObjectifId")]
        [InverseProperty("Prévisions")]
        public virtual Objectif? Objectif { get; set; } = null!;
        [InverseProperty("Prévision")]
        public virtual ICollection<Detail> Details { get; set; }
    }
}
