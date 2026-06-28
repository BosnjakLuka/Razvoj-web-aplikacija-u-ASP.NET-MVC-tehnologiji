// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using planinarenje.Data;
using planinarenje.Entiteti;

namespace planinarenje.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly PlaninarstvoDbContext _dbContext;

        public IndexModel(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            PlaninarstvoDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Korisničko ime mora imati između {2} i {1} znakova.")]
            [RegularExpression("^[a-zA-Z0-9_.]+$", ErrorMessage = "Korisničko ime smije sadržavati samo slova, brojeve, točku i donju crtu.")]
            [Display(Name = "Korisničko ime")]
            public string KorisnickoIme { get; set; }
        }

        private async Task LoadAsync(AppUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var korisnik = await _dbContext.Korisnici.FirstOrDefaultAsync(k => k.AppUserId == user.Id);

            Username = userName;

            Input = new InputModel
            {
                KorisnickoIme = korisnik?.KorisnickoIme
            };
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }

            var korisnik = await _dbContext.Korisnici.FirstOrDefaultAsync(k => k.AppUserId == user.Id);
            if (korisnik != null && korisnik.KorisnickoIme != Input.KorisnickoIme)
            {
                var korisnickoImeZauzeto = await _dbContext.Korisnici
                    .AnyAsync(k => k.KorisnickoIme == Input.KorisnickoIme && k.IdKorisnik != korisnik.IdKorisnik);
                if (korisnickoImeZauzeto)
                {
                    ModelState.AddModelError("Input.KorisnickoIme", "Korisničko ime je već zauzeto.");
                    await LoadAsync(user);
                    return Page();
                }

                korisnik.KorisnickoIme = Input.KorisnickoIme;
                await _dbContext.SaveChangesAsync();
            }

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
