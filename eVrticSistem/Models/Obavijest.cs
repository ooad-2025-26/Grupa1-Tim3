using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EVrtic.Models
{
    public class Obavijest
    {
        [Key]
        public int IdObavijesti { get; set; }

        [Required]
        [StringLength(150)]
        public string Naslov { get; set; } = string.Empty;

        [Required]
        [StringLength(2000)]
        public string Sadrzaj { get; set; } = string.Empty;

        public DateTime DatumKreiranja { get; set; } = DateTime.Now;

        public DateTime? DatumSlanja { get; set; }

        public KanalSlanja KanalSlanja { get; set; }

        public StatusObavijesti StatusObavijesti { get; set; } = StatusObavijesti.KREIRANA;

        public int RoditeljId { get; set; }

        [ForeignKey(nameof(RoditeljId))]
        public Roditelj? Roditelj { get; set; }

        public int? OdgajateljId { get; set; }

        [ForeignKey(nameof(OdgajateljId))]
        public Odgajatelj? Odgajatelj { get; set; }

        public Obavijest()
        {
        }
    }
}