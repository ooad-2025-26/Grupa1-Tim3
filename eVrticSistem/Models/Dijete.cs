using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EVrtic.Models
{
    public class Dijete
    {
        [Key]
        public int IdDjeteta { get; set; }

        [Required]
        [StringLength(100)]
        public string ImePrezime { get; set; } = string.Empty;

        [Required]
        [StringLength(30)]
        public string IdentifikacioniKod { get; set; } = string.Empty;

        [StringLength(500)]
        public string DodatnaNapomena { get; set; } = string.Empty;

        public bool Aktivno { get; set; } = true;

        public int? GrupaId { get; set; }

        [ForeignKey(nameof(GrupaId))]
        public Grupa? Grupa { get; set; }

        public int? RoditeljId { get; set; }

        [ForeignKey(nameof(RoditeljId))]
        public Roditelj? Roditelj { get; set; }

        public ICollection<AlergijaDjeteta> Alergije { get; set; } = new List<AlergijaDjeteta>();

        public ICollection<BolestDjeteta> Bolesti { get; set; } = new List<BolestDjeteta>();

        public ICollection<DnevniIzvjestaj> DnevniIzvjestaji { get; set; } = new List<DnevniIzvjestaj>();

        public ICollection<EvidencijaDolaskaOdlaska> EvidencijeDolaskaOdlaska { get; set; } = new List<EvidencijaDolaskaOdlaska>();

        public ICollection<SazetakAktivnosti> SazeciAktivnosti { get; set; } = new List<SazetakAktivnosti>();

        public Dijete()
        {
        }
    }
}