using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DropDown.Models.Cascade
{
    [Table("Projets")]
    public class Projet
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public Programme Programme { get; set; }
    }
}
