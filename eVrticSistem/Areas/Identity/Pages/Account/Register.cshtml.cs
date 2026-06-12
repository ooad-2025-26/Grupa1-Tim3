using EVrtic.Data;
using EVrtic.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EVrtic.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<Korisnik> _signInManager;
        private readonly UserManager<Korisnik> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<Korisnik> userManager,
            SignInManager<Korisnik> signInManager,
            ILogger<RegisterModel> logger,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; } = new InputModel();

        public string? ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; } = new List<AuthenticationScheme>();

        public class InputModel
        {
            [Required(ErrorMessage = "Ime i prezime je obavezno.")]
            [StringLength(100)]
            [Display(Name = "Ime i prezime")]
            public string ImePrezime { get; set; } = string.Empty;

            [Required(ErrorMessage = "Email je obavezan.")]
            [EmailAddress(ErrorMessage = "Unesite ispravnu email adresu.")]
            [Display(Name = "Email")]
            public string Email { get; set; } = string.Empty;

            [Required(ErrorMessage = "Lozinka je obavezna.")]
            [StringLength(100, ErrorMessage = "Lozinka mora imati najmanje {2} karaktera.", MinimumLength = 5)]
            [DataType(DataType.Password)]
            [Display(Name = "Lozinka")]
            public string Password { get; set; } = string.Empty;

            [DataType(DataType.Password)]
            [Display(Name = "Potvrda lozinke")]
            [Compare("Password", ErrorMessage = "Lozinka i potvrda lozinke se ne poklapaju.")]
            public string ConfirmPassword { get; set; } = string.Empty;

            [Required(ErrorMessage = "Uloga je obavezna.")]
            [Display(Name = "Uloga")]
            public Uloga? Uloga { get; set; }

            [Display(Name = "Identifikacioni kod djeteta")]
            public string? IdentifikacioniKodDjeteta { get; set; }

            [Display(Name = "Kontakt telefon")]
            [RegularExpression(@"^(03|06)\d{7}$", ErrorMessage = "Broj telefona mora imati tačno 9 cifara i počinjati sa 03 ili 06.")]
            public string? KontaktTelefon { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        private static bool ImePrezimeJeIspravno(string? imePrezime)
        {
            if (string.IsNullOrWhiteSpace(imePrezime))
                return false;

            var dijelovi = Regex.Split(imePrezime.Trim(), @"\s+");

            return dijelovi.Length >= 2 &&
                   dijelovi.All(dio => Regex.IsMatch(dio, @"^[A-ZČĆŽŠĐ][a-zčćžšđ]+$"));
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            Input.ImePrezime = Regex.Replace(Input.ImePrezime.Trim(), @"\s+", " ");
            Input.Email = Input.Email.Trim();
            Input.KontaktTelefon = string.IsNullOrWhiteSpace(Input.KontaktTelefon)
                ? null
                : Regex.Replace(Input.KontaktTelefon.Trim(), @"\s+", "");

            if (!ImePrezimeJeIspravno(Input.ImePrezime))
            {
                ModelState.AddModelError(
                    "Input.ImePrezime",
                    "Ime i prezime mora imati najmanje dvije riječi. Svaka riječ mora početi velikim slovom i sadržavati samo slova."
                );
            }

            if (Input.Uloga == Uloga.ADMINISTRATOR)
            {
                ModelState.AddModelError("Input.Uloga", "Administrator se ne može registrovati putem forme.");
            }

            var kodDjeteta = Input.IdentifikacioniKodDjeteta?.Trim() ?? string.Empty;

            if (Input.Uloga == Uloga.RODITELJ)
            {
                if (string.IsNullOrWhiteSpace(kodDjeteta))
                {
                    ModelState.AddModelError("Input.IdentifikacioniKodDjeteta", "Identifikacioni kod djeteta je obavezan za roditelja.");
                }
                else if (kodDjeteta.Length != 8)
                {
                    ModelState.AddModelError("Input.IdentifikacioniKodDjeteta", "Identifikacioni kod djeteta mora imati tačno 8 karaktera.");
                }
            }

            bool emailVecPostoji = await _userManager.FindByEmailAsync(Input.Email) != null;
            if (emailVecPostoji)
                ModelState.AddModelError("Input.Email", "Korisnik sa ovom email adresom već postoji.");

            Dijete? dijete = null;

            if (Input.Uloga == Uloga.RODITELJ && !string.IsNullOrWhiteSpace(kodDjeteta) && kodDjeteta.Length == 8)
            {
                dijete = await _context.Djeca
                    .FirstOrDefaultAsync(d =>
                        EF.Functions.Collate(d.IdentifikacioniKod, "Latin1_General_100_CS_AS") == kodDjeteta);

                if (dijete == null)
                {
                    ModelState.AddModelError("Input.IdentifikacioniKodDjeteta", "Dijete sa unesenim identifikacionim kodom nije pronađeno.");
                }
                else if (dijete.RoditeljId != null)
                {
                    ModelState.AddModelError("Input.IdentifikacioniKodDjeteta", "Dijete je već povezano sa roditeljem.");
                }
            }

            if (!ModelState.IsValid)
                return Page();

            Korisnik noviKorisnik;

            if (Input.Uloga == Uloga.RODITELJ)
            {
                noviKorisnik = new Roditelj
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    EmailConfirmed = true,
                    ImePrezime = Input.ImePrezime.Trim(),
                    StatusNaloga = StatusNaloga.AKTIVAN,
                    KontaktTelefon = Input.KontaktTelefon?.Trim()
                };
            }
            else
            {
                noviKorisnik = new Odgajatelj
                {
                    UserName = Input.Email,
                    Email = Input.Email,
                    EmailConfirmed = true,
                    ImePrezime = Input.ImePrezime.Trim(),
                    StatusNaloga = StatusNaloga.AKTIVAN
                };
            }

            var result = await _userManager.CreateAsync(noviKorisnik, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Korisnik je kreirao novi nalog.");
                await _userManager.AddToRoleAsync(noviKorisnik, Input.Uloga.ToString()!);

                if (Input.Uloga == Uloga.RODITELJ && dijete != null)
                {
                    dijete.RoditeljId = noviKorisnik.Id;
                    await _context.SaveChangesAsync();
                }

                await _signInManager.SignInAsync(noviKorisnik, isPersistent: false);

                if (Input.Uloga == Uloga.RODITELJ)
                    return RedirectToAction("UnosPodataka", "Dijete", new { novaRegistracija = true });

                returnUrl ??= Url.Content("~/Home/RedirectByRole");
                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return Page();
        }
    }
}
