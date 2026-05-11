using System.ComponentModel.DataAnnotations;

namespace EVrtic.Models
{
    public class Korisnik
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string ImePrezime { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public Uloga Uloga { get; set; }

        [Required]
        public StatusNaloga StatusNaloga { get; set; } = StatusNaloga.AKTIVAN;

        public Korisnik()
        {
        }
    }
}