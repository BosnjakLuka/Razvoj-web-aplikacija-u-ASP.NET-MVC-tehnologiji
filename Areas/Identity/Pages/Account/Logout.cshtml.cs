// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using planinarenje.Entiteti;
using planinarenje.Data;

namespace planinarenje.Areas.Identity.Pages.Account
{
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly PlaninarstvoDbContext _dbContext;
        private readonly ILogger<LogoutModel> _logger;

        public LogoutModel(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager, PlaninarstvoDbContext dbContext, ILogger<LogoutModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<IActionResult> OnPost(string returnUrl = null)
        {
            // Korisnika moramo pročitati prije odjave - nakon SignOutAsync principal više nije dostupan.
            var appUserId = _userManager.GetUserId(User);
            var korisnickoIme = User?.Identity?.Name;

            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            await _dbContext.ZabiljeziAuthDogadajAsync(TipAkcijeLoga.Odjava, appUserId, korisnickoIme, "Odjava korisnika");
            if (returnUrl != null)
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                // This needs to be a redirect so that the browser performs a new
                // request and the identity for the user gets updated.
                return RedirectToPage();
            }
        }
    }
}
