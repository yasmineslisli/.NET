using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DropDown.Models
{
    public partial class Exercice
    {
        public Exercice()
        {
            Objectifs = new HashSet<Objectif>();
        }

        [Key]
        public int Id { get; set; }
        public string Annee { get; set; } = null!;

        [InverseProperty("Exercice")]
        public virtual ICollection<Objectif> Objectifs { get; set; }
    }
}
