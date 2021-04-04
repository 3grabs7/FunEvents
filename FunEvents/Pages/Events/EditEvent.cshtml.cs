using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FunEvents.Data;
using FunEvents.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FunEvents.Pages.Events
{
    [Authorize(Roles = "Admin, Organizer")]
    public class EditEventModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ActiveUser> _userManager;

        public EditEventModel(ApplicationDbContext context, UserManager<ActiveUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public ActiveUser ActiveUser { get; set; }
        [BindProperty]
        public IList<Event> Events { get; set; }
        [BindProperty]
        public Event Event { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string userId = _userManager.GetUserId(User);

            Events = await _context.Events.Where(e => e.Organizer.ActiveUser.Id == userId).ToListAsync();

            return Page();
        }


        // Funkar inte som det ska ännu
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.AttachRange(Events);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EventExists(Event.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
