using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DropDown.Models.Cascade
{
    [Table("Programmes")]

    public class Programme
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
