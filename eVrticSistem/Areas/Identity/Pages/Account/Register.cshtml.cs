using EVrtic.Data;
using EVrtic.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

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
            [StringLength(20, ErrorMessage = "Broj telefona može imati najviše 20 karaktera.")]
            public string? KontaktTelefon { get; set; }
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (Input.Uloga == Uloga.ADMINISTRATOR)
                ModelState.AddModelError("Input.Uloga", "Administrator se ne može registrovati putem forme.");

            if (Input.Uloga == Uloga.RODITELJ && string.IsNullOrWhiteSpace(Input.IdentifikacioniKodDjeteta))
                ModelState.AddModelError("Input.IdentifikacioniKodDjeteta", "Identifikacioni kod djeteta je obavezan za roditelja.");

            bool emailVecPostoji = await _userManager.FindByEmailAsync(Input.Email) != null;
            if (emailVecPostoji)
                ModelState.AddModelError("Input.Email", "Korisnik sa ovom email adresom već postoji.");

            Dijete? dijete = null;

            if (Input.Uloga == Uloga.RODITELJ && !string.IsNullOrWhiteSpace(Input.IdentifikacioniKodDjeteta))
            {
                dijete = await _context.Djeca
                    .FirstOrDefaultAsync(d => d.IdentifikacioniKod == Input.IdentifikacioniKodDjeteta.Trim());

                if (dijete == null)
                    ModelState.AddModelError("Input.IdentifikacioniKodDjeteta", "Dijete sa unesenim identifikacionim kodom nije pronađeno.");
                else if (dijete.RoditeljId != null)
                    ModelState.AddModelError("Input.IdentifikacioniKodDjeteta", "Dijete je već povezano sa roditeljem.");
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
                    ImePrezime = Input.ImePrezime,
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
                    ImePrezime = Input.ImePrezime,
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
