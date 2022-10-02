using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DropDown.Models
{
    public partial class Programme
    {
        public Programme()
        {
            Projets = new HashSet<Projet>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        [InverseProperty("Programme")]
        public virtual ICollection<Projet> Projets { get; set; }
    }
}
