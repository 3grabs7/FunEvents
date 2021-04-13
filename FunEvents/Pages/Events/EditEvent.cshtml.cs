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
    [Authorize(Roles = "Admin, OrganizerManager, OrganizerAssistant")]
    public class EditEventModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<EditEventModel> _logger;

        public EditEventModel(ApplicationDbContext context,
            UserManager<AppUser> userManager,
            ILogger<EditEventModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public IList<Organizer> Organizers { get; set; }
        [BindProperty]
        public IList<Event> Events { get; set; }
        [BindProperty]
        public Event Event { get; set; }
        public IList<Event> EventsWhereUserIsManager { get; set; }
        public IList<Event> EventsWhereUserIsAssistant { get; set; }

        public bool HasEventBeenSelectedForEdit { get; set; }
        public bool EditSucceeded { get; set; }
        public bool EditFailed { get; set; }

        public async Task OnGetAsync(int? selectedEvent)
        {
            Event = _context.Events.Find(selectedEvent);
            HasEventBeenSelectedForEdit = selectedEvent == null ? false : true;
            string userId = _userManager.GetUserId(User);

            EventsWhereUserIsManager = await _context.Events
                .Include(e => e.Organizer)
                .Where(e => e.Organizer.OrganizerManagers.Any(m => m.Id == userId))
                .ToListAsync();

            EventsWhereUserIsAssistant = await _context.Events
                .Include(e => e.Organizer)
                .Where(e => e.Organizer.OrganizerAssistants.Any(a => a.Id == userId))
                .ToListAsync();

        }

        public async Task<IActionResult> OnPostEditAsync(int? id)
        {
            Event = await _context.Events.FindAsync(id);

            if (Event == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Event>(Event, "event",
                s => s.Title, s => s.Description, s => s.Date, s => s.Place, s => s.Address, s => s.SpotsAvailable))
            {
                if (Event.SpotsAvailable < 0)
                {
                    EditFailed = true;
                    return Page();
                }
                else
                {
                    EditSucceeded = true;
                    await _context.SaveChangesAsync();
                    return Page();
                }
            }

            EditFailed = true;
            return Page();
        }

        public async Task<IActionResult> OnPostRequestEditAsync(int? id)
        {
            var requestForEvent = await _context.Events.FindAsync(id);
            requestForEvent.EventChangesPendingManagerValidation.Add(Event);
            await _context.SaveChangesAsync();

            return Page();
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
    }
}
