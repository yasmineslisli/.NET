using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DropDown.Models
{
    public partial class Structure
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
    }
}
