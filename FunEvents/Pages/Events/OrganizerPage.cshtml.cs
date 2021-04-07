using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FunEvents.Data;
using FunEvents.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FunEvents.Pages.Events
{
    public class OrganizerPageModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrganizerPageModel(ApplicationDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Event> Events { get; set; }
        public AppUser Organizer { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (String.IsNullOrWhiteSpace(id))
            {
                // Post alert about organizer not having a page set up yet
                // (Events are seeded without organizers)
                return RedirectToPage("/Errors/NotFound");
            }

            Organizer = await _context.Users.FindAsync(id);

            Events = await _context.Events
                .Where(e => e.Organizer.Id == id)
                .ToListAsync();

            return Page();
        }
    }
}
