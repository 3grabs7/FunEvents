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
using Microsoft.Extensions.Logging;

namespace FunEvents.Pages.Events
{
    [Authorize(Roles = "Admin, Organizer")]
    public class EditEventModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<EditEventModel> _logger;

        public EditEventModel(ApplicationDbContext context, UserManager<AppUser> userManager, ILogger<EditEventModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public AppUser AppUser { get; set; }
        [BindProperty]
        public IList<Event> Events { get; set; }
        [BindProperty]
        public Event Event { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string userId = _userManager.GetUserId(User);
            Events = await _context.Events.Where(e => e.Organizer.Id == userId).ToListAsync();

            return Page();
        }


        // Funkar inte som det ska ännu
        public async Task<IActionResult> OnPostSaveAsync(int? id)
        {
            var eventToUpdate = await _context.Events.FindAsync(id);

            if(eventToUpdate == null)
            {
                return NotFound();
            }

            if(await TryUpdateModelAsync<Event>(eventToUpdate, "event",
                s => s.Title, s => s.Description, s => s.Date, s => s.Place, s => s.Address, s => s.SpotsAvailable))
            {
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();

            //_context.AttachRange(Events);

            //try
            //{
            //    await _context.SaveChangesAsync();
            //}
            //catch (Exception exception)
            //{
            //    Console.WriteLine(exception.Message); // Tillfällig lösning. Bör loggas korrekt
            //}
            //return Page();
        }

        public async Task<IActionResult> OnPostCancelAsync(int? id)
        {
            var eventToDelete = await _context.Events.FindAsync(id);

            if (eventToDelete == null)
            {
                return NotFound();
            }

            try
            {
                _context.Events.Remove(eventToDelete);
                await _context.SaveChangesAsync();
                return RedirectToPage("./Index");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "ERROR! Could not cancel event.");

                return RedirectToPage("./Index");
            }
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
