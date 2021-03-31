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
        private readonly UserManager<ActiveUser> _userManager;
        private readonly SignInManager<ActiveUser> _signInManager;

        public CreateEventModel(ApplicationDbContext context,
           UserManager<ActiveUser> userManager,
           SignInManager<ActiveUser> signInManager)
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

            string organizerId = _userManager.GetUserId(User);
            Organizer TempOrganizer = new Organizer()
            {
                Name = "Default",
                ActiveUser = await _context.Users.Where(u => u.Id == organizerId).FirstOrDefaultAsync()
            };

            NewEvent.Organizer = TempOrganizer;
            _context.Events.Add(NewEvent);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
