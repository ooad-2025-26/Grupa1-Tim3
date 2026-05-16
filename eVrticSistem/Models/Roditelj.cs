namespace EVrtic.Models
{
    public class Roditelj : Korisnik
    {
        public ICollection<Dijete> Djeca { get; set; } = new List<Dijete>();

        public Roditelj()
        {
            Uloga = Uloga.RODITELJ;
        }
    }
}
