#nullable disable

using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using EVrtic.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace eVrticSistem.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<Korisnik> _userManager;

        public ResetPasswordModel(UserManager<Korisnik> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        // Email naloga za koji se mijenja lozinka — samo za prikaz (ne unosi se)
        public string Email { get; private set; }

        public class InputModel
        {
            [Required]
            public string UserId { get; set; }

            [Required]
            public string Code { get; set; }

            [Required(ErrorMessage = "Lozinka je obavezna.")]
            [StringLength(100, ErrorMessage = "Lozinka mora imati najmanje {2} karaktera.", MinimumLength = 5)]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Potvrda lozinke")]
            [Compare(nameof(Password), ErrorMessage = "Lozinka i potvrda lozinke se ne podudaraju.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string code = null, string userId = null)
        {
            if (code == null || userId == null)
            {
                return BadRequest("Link za resetovanje lozinke nije ispravan.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            Email = user?.Email ?? string.Empty;

            Input = new InputModel
            {
                Code = code,
                UserId = userId
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(Input.UserId);
            Email = user?.Email ?? string.Empty;

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (user == null)
            {
                // Ne otkrivamo da nalog ne postoji.
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(Input.Code));
            var result = await _userManager.ResetPasswordAsync(user, token, Input.Password);

            if (result.Succeeded)
            {
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }
    }
}
