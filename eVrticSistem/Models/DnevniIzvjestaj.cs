
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EVrtic.Models
{
    public class DnevniIzvjestaj
    {
        [Key]
        public int Id { get; set; }

        public DateTime Datum { get; set; }

        // ── Doručak ──
        [StringLength(200)]
        [Display(Name = "Doručak")]
        public string Dorucak { get; set; } = string.Empty;

        [Display(Name = "Status doručka")]
        public StatusObroka StatusDorucka { get; set; }

        // ── Ručak ──
        [StringLength(200)]
        [Display(Name = "Ručak")]
        public string Rucak { get; set; } = string.Empty;

        [Display(Name = "Status ručka")]
        public StatusObroka StatusRucka { get; set; }

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
