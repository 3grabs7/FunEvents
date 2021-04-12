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
    [Authorize(Roles = "Admin, Organizer, OrganizerManager, OrganizerAssistant")]
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
        public AppUser AppUser { get; set; }
        [BindProperty]
        public IList<Organizer> Organizers { get; set; }
        [BindProperty]
        public IList<Event> Events { get; set; }
        [BindProperty]
        public Event Event { get; set; }
        public IList<Event> EventsWhereUserIsManager { get; set; }
        public IList<Event> EventsWhereUserIsAssistant { get; set; }

        public bool eventSelected { get; set; }
        public bool editSuccess { get; set; }
        public bool editFailed { get; set; }

        public async Task<IActionResult> OnGetAsync(int? selectedEvent)
        {
            Event = _context.Events.Find(selectedEvent);
            eventSelected = selectedEvent == null ? false : true;
            string userId = _userManager.GetUserId(User);

            EventsWhereUserIsManager = await _context.Events.Where(e => e.EventOrganizer.OrganizerManagers.Any(m => m.Id == userId)).ToListAsync();

            EventsWhereUserIsAssistant = await _context.Events.Where(e => e.EventOrganizer.OrganizerAssistants.Any(a => a.Id == userId)).ToListAsync();


            //Events = await _context.Events.Include(e => e.Organizer).Where(e => e.Organizer.Id == userId).ToListAsync();

            AppUser = await _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.HostedEvents)
                .FirstOrDefaultAsync();

            // Ignorera detta än så länge
            // Organizers = await _context.Organizers.Where(o => o.)

            // Organizers = await _context.Organizers.Where(o => o.OrganizerAssistants.Contains(AppUser)).ToListAsync();

            foreach (var organizer in Organizers)
            {
                //if (AppUser.AssistantInOrganizations.Contains(organizer) {

                //}
            }
            

            return Page();
        }

        public async Task<IActionResult> OnPostEditAsync(int? id, int? selectedEvent)
        {
            Event = _context.Events.Find(selectedEvent);
            eventSelected = selectedEvent == null ? false : true;

            var eventToUpdate = await _context.Events
                .Include(e => e.Organizer)
                .FirstOrDefaultAsync(e => e.Id == id);

            string userId = _userManager.GetUserId(User);

            AppUser = await _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.HostedEvents)
                .FirstOrDefaultAsync();

            Events = await _context.Events
                .Include(e => e.Organizer)
                .Where(e => e.Organizer.Id == userId)
                .ToListAsync();

            if (eventToUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Event>(eventToUpdate, "event",
                s => s.Title, s => s.Description, s => s.Date, s => s.Place, s => s.Address, s => s.SpotsAvailable))
            {
                if (eventToUpdate.SpotsAvailable < 0)
                {
                    editFailed = true;
                    return Page();
                }
                else
                {
                    editSuccess = true;
                    await _context.SaveChangesAsync();
                    return Page();
                }
            }

            editFailed = true;
            return Page();
        }

        public async Task<IActionResult> OnPostRequestEditAsync(int? id)
        {

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
