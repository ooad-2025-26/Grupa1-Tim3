using System.ComponentModel.DataAnnotations;

namespace EVrtic.Models
{
    public class DnevniQRCode
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string VrijednostKoda { get; set; } = string.Empty;

        public DateTime DatumVazenja { get; set; }

        public DateTime VrijemeIsteka { get; set; }

        public bool Aktivan { get; set; } = true;

        public ICollection<EvidencijaDolaskaOdlaska> Evidencije { get; set; } = new List<EvidencijaDolaskaOdlaska>();

        public DnevniQRCode()
        {
        }
    }
}