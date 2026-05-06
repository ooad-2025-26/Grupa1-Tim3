using eVrtic.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EVrtic.Models
{
    public class EvidencijaDolaskaOdlaska
    {
        [Key]
        public int Id { get; set; }

        public DateTime VrijemeDogadjaja { get; set; } = DateTime.Now;

        public TipDogadjaja TipDogadjaja { get; set; }

        [Required]
        [StringLength(30)]
        public string UneseniKodDjeteta { get; set; } = string.Empty;

        public bool ValidanQRKod { get; set; }

        public bool KodDjetetaIspravan { get; set; }

        public StatusEvidencije StatusEvidencije { get; set; }

        public int DijeteId { get; set; }

        [ForeignKey(nameof(DijeteId))]
        public Dijete? Dijete { get; set; }

        public int? DnevniQRCodeId { get; set; }

        [ForeignKey(nameof(DnevniQRCodeId))]
        public DnevniQRCode? DnevniQRCode { get; set; }

        public EvidencijaDolaskaOdlaska()
        {
        }
    }
}