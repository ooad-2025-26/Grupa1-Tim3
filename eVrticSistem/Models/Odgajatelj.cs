namespace EVrtic.Models
{
    public class Odgajatelj : Korisnik
    {
        public ICollection<Grupa> Grupe { get; set; } = new List<Grupa>();

        public Odgajatelj()
        {
            Uloga = Uloga.ODGAJATELJ;
        }
    }
}
