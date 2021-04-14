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
        public IList<Organization> Organizations { get; set; }
        [BindProperty]
        public IList<Event> Events { get; set; }
        [BindProperty]
        public Event Event { get; set; }
        public IList<Event> EventsWhereUserIsManager { get; set; }
        public IList<Event> EventsWhereUserIsAssistant { get; set; }

        public bool HasEventBeenSelectedForEdit { get; set; }
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
            HasEventBeenSelectedForEdit = selectedEvent == null ? false : true;

            string userId = _userManager.GetUserId(User);

            EventsWhereUserIsManager = await _context.Events
                .Include(e => e.Organization)
                .Where(e => e.Organization.OrganizationManagers.Any(m => m.Id == userId))
                .ToListAsync();

            EventsWhereUserIsAssistant = await _context.Events
                .Include(e => e.Organization)
                .Where(e => e.Organization.OrganizationAssistants.Any(a => a.Id == userId))
                .ToListAsync();

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
            var shadowEvent = new Event() { Id = 999999 };
            var reflections = Event.GetType().GetProperties();
            foreach (var reflection in reflections)
            {
                if (reflection.Name == "Id") continue;
                shadowEvent.GetType()
                    .GetProperty(reflection.Name)
                    .SetValue(shadowEvent, reflection.GetValue(Event, null));
            }

            try
            {
                Event.EventChangesPendingManagerValidation.Add(shadowEvent);
                await _context.SaveChangesAsync();
                return RedirectToPage("/Events/EditEvent", new { EditSucceeded = true });
            }
            catch (Exception e)
            {
                // throw new Exception(e.Message);
                return RedirectToPage("/Events/EditEvent", new { EditFailed = true });
            }
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
