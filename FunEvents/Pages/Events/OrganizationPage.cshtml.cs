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
    public class OrganizationPageModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrganizationPageModel(ApplicationDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Event> Events { get; set; }
        public Organization Organization { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                // Post alert about organizer not having a page set up yet
                // (Events are seeded without organizers)
                return RedirectToPage("/Errors/NotFound");
            }

            Organization = await _context.Organizations.FindAsync(id);

            Events = await _context.Events
                .Where(e => e.Organization.Id == id)
                .ToListAsync();

            return Page();
        }
    }
}