using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DropDown.Models
{
    [Table("DDs")]
    public partial class Dd
    {
        public Dd()
        {
            Details = new HashSet<Detail>();
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        [InverseProperty("Dd")]
        public virtual ICollection<Detail> Details { get; set; }
    }
}
