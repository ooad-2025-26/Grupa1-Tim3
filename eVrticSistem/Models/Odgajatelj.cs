using System.Text.RegularExpressions;

namespace EVrtic.Models
{
    public class Odgajatelj : Korisnik
    {
        public ICollection<Grupa> Grupe { get; set; } = new List<Grupa>();

        public Odgajatelj()
        {
            Uloga = eVrtic.Models.Uloga.ODGAJATELJ;
        }
    }
}