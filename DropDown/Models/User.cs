using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace DropDown.Models
{
    
    public partial class User
    {
        [Key]
        public int Id { get; set; }
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        [Column("CIN")]
        public string? Cin { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? ConfirmPassword { get; set; }
        public int? StructureId { get; set; }
        public virtual Structure? structure { get; set; }

        public int? ProfilId { get; set; }
        public virtual Profil? Profil { get; set; }
        public string? Statut { get; set; }
        public string? Role { get; set; }
    }
}
