using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FunEvents.Data;
using FunEvents.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FunEvents.Pages.Events
{
    [Authorize(Roles = "Admin, Organizer")]
    public class CreateEventModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public CreateEventModel(ApplicationDbContext context,
           UserManager<AppUser> userManager,
           SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public Event NewEvent { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            NewEvent.Organizer = await _context.Users.FindAsync(_userManager.GetUserId(User));
            NewEvent.CreatedAt = DateTime.Now;
            await _context.Events.AddAsync(NewEvent);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
