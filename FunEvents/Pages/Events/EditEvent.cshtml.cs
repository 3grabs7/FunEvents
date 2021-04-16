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
using System.Reflection;

namespace FunEvents.Pages.Events
{
    [Authorize(Roles = "Admin, OrganizationManager, OrganizationAssistant")]
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
        public Event Event { get; set; }
        public IList<Event> EventsWhereUserIsManager { get; set; }
        public IList<Event> EventsWhereUserIsAssistant { get; set; }
        public IEnumerable<IGrouping<Organization, ShadowEvent>> EventsPendingEditRequest { get; set; }

        public bool EditSucceeded { get; set; }
        public bool EditFailed { get; set; }

        public async Task OnGetAsync(int? selectedEvent,
           bool? editSucceeded,
           bool? editFailed)
        {
            EditSucceeded = editSucceeded ?? false;
            EditFailed = editFailed ?? false;

            Event = await _context.Events
                .Include(e => e.EventChangesPendingManagerValidation)
                .FirstOrDefaultAsync(e => e.Id == selectedEvent);

            string userId = _userManager.GetUserId(User);

            EventsWhereUserIsManager = await _context.Events
                .Include(e => e.Organization)
                .Include(e => e.EventChangesPendingManagerValidation)
                .Where(e => e.Organization.OrganizationManagers.Any(m => m.Id == userId))
                .ToListAsync();

            EventsWhereUserIsAssistant = await _context.Events
                .Include(e => e.Organization)
                .Where(e => e.Organization.OrganizationAssistants.Any(a => a.Id == userId))
                .ToListAsync();

            EventsPendingEditRequest = EventsWhereUserIsManager
                .SelectMany(e => e.EventChangesPendingManagerValidation)
                .GroupBy(g => g.PendingEditEvent.Organization)
                .AsEnumerable();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            try
            {
                _context.Attach(Event).State = EntityState.Modified;
                EditSucceeded = true;
                await _context.SaveChangesAsync();
                return RedirectToPage("/Events/EditEvent", new { EditSucceeded = true });
            }
            catch
            {
                return RedirectToPage("/Events/EditEvent", new { EditFailed = true });
            }
        }

        public async Task<IActionResult> OnPostRequestEditAsync()
        {
            var shadowEvent = new ShadowEvent()
            {
                PendingEditEvent = _context.Events.Find(Event.Id),
                Editor = _context.Users.Find(_userManager.GetUserId(User))
            };

            var reflections = new EditableEvent().GetType().GetProperties();
            foreach (var reflection in reflections)
            {
                if (reflection.Name == "Id") continue;
                reflection.SetValue(shadowEvent, reflection.GetValue(Event));
            }

            try
            {
                await _context.ShadowEvents.AddAsync(shadowEvent);
                await _context.SaveChangesAsync();
                return RedirectToPage("/Events/EditEvent", new { EditSucceeded = true });
            }
            catch
            {
                return RedirectToPage("/Events/EditEvent", new { EditFailed = true });
            }
        }

        public async Task<IActionResult> OnPostCancelAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eventToDelete = await _context.Events.FindAsync(id);
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
