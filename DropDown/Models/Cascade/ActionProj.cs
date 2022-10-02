using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DropDown.Models.Cascade
{
    [Table("ActionProjs")]

    public class ActionProj
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public Projet Projet { get; set; }
    }
}
