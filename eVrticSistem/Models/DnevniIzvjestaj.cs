using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EVrtic.Models
{
    public class DnevniIzvjestaj
    {
        [Key]
        public int IdIzvjestaja { get; set; }

        public DateTime Datum { get; set; }

        [StringLength(100)]
        public string Obrok { get; set; } = string.Empty;

        public StatusObroka StatusObroka { get; set; }

        public int SpavanjeMinuta { get; set; }

        public TimeSpan? VrijemeDolaska { get; set; }

        public TimeSpan? VrijemeOdlaska { get; set; }

        [StringLength(1000)]
        public string NapomenaAktivnosti { get; set; } = string.Empty;

        public DateTime DatumKreiranja { get; set; } = DateTime.Now;

        public int DijeteId { get; set; }

        [ForeignKey(nameof(DijeteId))]
        public Dijete? Dijete { get; set; }

        public DnevniIzvjestaj()
        {
        }
    }
}
