using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace EVrtic.Models
{
    public class Korisnik : IdentityUser<int>
    {
        // Id (int), UserName, Email, PasswordHash itd. dolaze iz IdentityUser<int>
        // Email je već u IdentityUser — uklonjen je odavde

        [Required]
        [StringLength(100)]
        public string ImePrezime { get; set; } = string.Empty;

        [Required]
        public Uloga Uloga { get; set; }

        [Required]
        public StatusNaloga StatusNaloga { get; set; } = StatusNaloga.AKTIVAN;

        public Korisnik()
        {
        }
    }
}
