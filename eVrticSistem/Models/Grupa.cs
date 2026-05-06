using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EVrtic.Models
{
    public class Grupa
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ImeGrupe { get; set; } = string.Empty;

        public int? OdgajateljId { get; set; }

        [ForeignKey(nameof(OdgajateljId))]
        public Odgajatelj? Odgajatelj { get; set; }

        public ICollection<Dijete> Djeca { get; set; } = new List<Dijete>();

        public Grupa()
        {
        }
    }
}