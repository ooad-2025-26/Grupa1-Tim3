using System.ComponentModel.DataAnnotations;

namespace EVrtic.Models
{
    public class Roditelj : Korisnik
    {
        [StringLength(20)]
        [Display(Name = "Kontakt telefon")]
        public string? KontaktTelefon { get; set; }

        public ICollection<Dijete> Djeca { get; set; } = new List<Dijete>();

        public Roditelj()
        {
            Uloga = Uloga.RODITELJ;
        }
    }
}
