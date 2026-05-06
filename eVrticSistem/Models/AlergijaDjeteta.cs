using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EVrtic.Models
{
    public class AlergijaDjeteta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Naziv { get; set; } = string.Empty;

        public int DijeteId { get; set; }

        [ForeignKey(nameof(DijeteId))]
        public Dijete? Dijete { get; set; }

        public AlergijaDjeteta()
        {
        }
    }
}