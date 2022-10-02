using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DropDown.Models
{
    [Table("DRs")]
    public partial class Dr
    {
        public Dr()
        {
            Objectifs = new HashSet<Objectif>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        [InverseProperty("Dr")]
        public virtual ICollection<Objectif> Objectifs { get; set; }
    }
}
