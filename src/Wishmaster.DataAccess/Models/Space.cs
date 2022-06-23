using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wishmaster.DataAccess.Models
{
    [Table("Spaces")]
    public class Space
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime Created { get; set; }
    }
}
