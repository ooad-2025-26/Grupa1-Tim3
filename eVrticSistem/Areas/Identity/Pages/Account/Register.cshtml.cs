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
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly ApplicationDbContext _context;

        public RegisterModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
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
            [StringLength(100, ErrorMessage = "Ime i prezime može imati najviše 100 karaktera.")]
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
        }

        public async Task OnGetAsync(string? returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Home/RedirectByRole");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (Input.Uloga == Uloga.ADMINISTRATOR)
            {
                ModelState.AddModelError("Input.Uloga", "Administrator se ne može registrovati putem forme.");
            }

            if (Input.Uloga == Uloga.RODITELJ && string.IsNullOrWhiteSpace(Input.IdentifikacioniKodDjeteta))
            {
                ModelState.AddModelError("Input.IdentifikacioniKodDjeteta", "Identifikacioni kod djeteta je obavezan za roditelja.");
            }

            bool emailVecPostojiUIdentity = await _userManager.FindByEmailAsync(Input.Email) != null;
            bool emailVecPostojiUDomenskimKorisnicima = await _context.Korisnici.AnyAsync(k => k.Email == Input.Email);

            if (emailVecPostojiUIdentity || emailVecPostojiUDomenskimKorisnicima)
            {
                ModelState.AddModelError("Input.Email", "Korisnik sa ovom email adresom već postoji.");
            }

            Dijete? dijete = null;

            if (Input.Uloga == Uloga.RODITELJ && !string.IsNullOrWhiteSpace(Input.IdentifikacioniKodDjeteta))
            {
                dijete = await _context.Djeca
                    .FirstOrDefaultAsync(d => d.IdentifikacioniKod == Input.IdentifikacioniKodDjeteta);

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
            {
                return Page();
            }

            var identityUser = new IdentityUser
            {
                UserName = Input.Email,
                Email = Input.Email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, Input.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("Korisnik je kreirao novi nalog.");

                if (Input.Uloga == Uloga.RODITELJ)
                {
                    var roditelj = new Roditelj
                    {
                        ImePrezime = Input.ImePrezime,
                        Email = Input.Email,
                        Uloga = Uloga.RODITELJ,
                        StatusNaloga = StatusNaloga.AKTIVAN
                    };

                    _context.Roditelji.Add(roditelj);
                    await _context.SaveChangesAsync();

                    if (dijete != null)
                    {
                        dijete.RoditeljId = roditelj.Id;
                        await _context.SaveChangesAsync();
                    }

                    await _userManager.AddToRoleAsync(identityUser, "RODITELJ");
                }
                else if (Input.Uloga == Uloga.ODGAJATELJ)
                {
                    var odgajatelj = new Odgajatelj
                    {
                        ImePrezime = Input.ImePrezime,
                        Email = Input.Email,
                        Uloga = Uloga.ODGAJATELJ,
                        StatusNaloga = StatusNaloga.AKTIVAN
                    };

                    _context.Odgajatelji.Add(odgajatelj);
                    await _context.SaveChangesAsync();

                    await _userManager.AddToRoleAsync(identityUser, "ODGAJATELJ");
                }

                await _signInManager.SignInAsync(identityUser, isPersistent: false);

                return LocalRedirect(returnUrl);
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}