#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using EVrtic.Models;
using EVrtic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace eVrticSistem.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<Korisnik> _userManager;
        private readonly IEmailService _emailService;

        public ForgotPasswordModel(UserManager<Korisnik> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email je obavezan.")]
            [EmailAddress(ErrorMessage = "Unesite ispravnu email adresu.")]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);

            // Iz sigurnosnih razloga ne otkrivamo da li nalog postoji —
            // u svakom slučaju vodimo na potvrdu.
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code, userId = user.Id },
                    protocol: Request.Scheme);

                var subject = "eVrtić — Resetovanje lozinke";
                var body =
                    "Poštovani,\n\n" +
                    "Zatražili ste promjenu lozinke za vaš eVrtić nalog.\n" +
                    "Otvorite sljedeći link da postavite novu lozinku:\n\n" +
                    callbackUrl + "\n\n" +
                    "Ako niste vi zatražili promjenu lozinke, slobodno zanemarite ovaj email.\n\n" +
                    "Srdačan pozdrav,\neVrtić tim";

                await _emailService.SendAsync(Input.Email, subject, body);
            }

            return RedirectToPage("./ForgotPasswordConfirmation");
        }
    }
}
