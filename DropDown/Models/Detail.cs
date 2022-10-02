using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DropDown.Models
{
    [Index("Ddid", Name = "IX_Details_DDId")]
    [Index("PrévisionId", Name = "IX_Details_PrévisionId")]
    public partial class Detail
    {
        [Key]
        public int Id { get; set; }
        [Column("numDossier")]
        public int? NumDossier { get; set; }
        [Column("TRN")]
        public string Trn { get; set; } = null!;
        [Column("indiceTRN")]
        public string IndiceTrn { get; set; } = null!;
        [Column("numTRN")]
        public int NumTrn { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Veuillez entrer une valeur supérieure à 0")]
        public int Superficie { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Veuillez entrer une valeur supérieure à 0")]
        public int? Valeur { get; set; }
        [Column("DDId")]
        public int Ddid { get; set; }
        public int PrévisionId { get; set; }

        [ForeignKey("Ddid")]
        [InverseProperty("Details")]
        public virtual Dd? Dd { get; set; } = null!;
        [ForeignKey("PrévisionId")]
        [InverseProperty("Details")]
        public virtual Prévision? Prévision { get; set; } = null!;
    }
}
