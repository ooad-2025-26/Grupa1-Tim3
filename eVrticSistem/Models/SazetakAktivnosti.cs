using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EVrtic.Models
{
    public class SazetakAktivnosti
    {
        [Key]
        public int Id { get; set; }

        public DateTime DatumPocetka { get; set; }

        public DateTime DatumKraja { get; set; }

        public TipSazetka TipSazetka { get; set; }

        public int BrojObroka { get; set; }

        public int BrojDolazaka { get; set; }

        public int AgregiranoSpavanjeMinuta { get; set; }

        [StringLength(2000)]
        public string OsnovneNapomene { get; set; } = string.Empty;

        public DateTime DatumGenerisanja { get; set; } = DateTime.Now;

        public int DijeteId { get; set; }

        [ForeignKey(nameof(DijeteId))]
        public Dijete? Dijete { get; set; }

        public SazetakAktivnosti()
        {
        }
    }
}