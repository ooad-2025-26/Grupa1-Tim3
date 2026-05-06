using eVrtic.Models;

namespace EVrtic.Models
{
    public class Administrator : Korisnik
    {
        public Administrator()
        {
            Uloga = Uloga.ADMINISTRATOR;
        }
    }
}