using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FunEvents.Data;
using FunEvents.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FunEvents.Pages.Events
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IHttpContextAccessor _accessor;

        public DetailsModel(ApplicationDbContext context,
           UserManager<AppUser> userManager,
           SignInManager<AppUser> signInManager,
           IHttpContextAccessor accessor)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _accessor = accessor;
        }

        public Event Event { get; set; }
        public AppUser AppUser { get; set; }
        public bool SucceededToJoinEvent { get; set; }
        public bool FailedToJoinEvent { get; set; }
        public bool RedirectedFromLogin { get; set; }
        public List<string> Attendees { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id,
            bool? succeededToJoinEvent,
            bool? failedToJoinEvent,
            bool? redirectedFromLogin)
        {
            if (id == null)
            {
                return RedirectToPage("/Errors/NotFound");
            }

            // Check if page was loaded with any prompts to display alerts
            SucceededToJoinEvent = succeededToJoinEvent ?? false;
            FailedToJoinEvent = failedToJoinEvent ?? false;

            Event = await _context.Events.FindAsync(id);
            Attendees = GetAttendeeInfo();
            AppUser = await GetAppuser(_userManager.GetUserId(User));

            if (redirectedFromLogin ?? false)
            {
                await JoinEventOnRedirectFromLogin(id);

                return Redirect($"/Events/Details?id={id}");
            }

            string clientIpAddress = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            if (!_context.Analytics.AnyAsync(a => a.Event.Id == id && a.Ip == clientIpAddress).Result)
            {
                Event.UniquePageVisits++;

            }
            Event.PageVisits++;
            await _context.SaveChangesAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("/Errors/NotFound");
            }

            Event = await _context.Events.FindAsync(id);
            AppUser = await GetAppuser(_userManager.GetUserId(User));

            try
            {
                AppUser.JoinedEvents.Add(Event);
                Event.SpotsAvailable--;

                await _context.SaveChangesAsync();
            }
            catch
            {
                return RedirectToPage("/Events/Details", new { id = id, failedToJoinEvent = true });
            }

            return RedirectToPage("/Events/Details", new { id = id, succeededToJoinEvent = true });
        }

        public int AttendeesCount() => _context.Events
            .Include(e => e.Attendees)
            .Where(e => e.Id == Event.Id)
            ?.First().Attendees.Count
            ?? 0;

        public List<string> GetAttendeeInfo()
        {
            var attendees = _context.Events
                .Include(e => e.Attendees)
                .Where(e => e.Id == Event.Id)
                .First().Attendees;

            return attendees?.Select(a => a.UserName).ToList() ?? new List<string> { "Couldn't Load Users" };
        }

        public async Task JoinEventOnRedirectFromLogin(int? id)
        {
            Event = await _context.Events.FindAsync(id);
            AppUser = await GetAppuser(_userManager.GetUserId(User));
            AppUser.JoinedEvents.Add(Event);

            Event.SpotsAvailable--;

            await _context.SaveChangesAsync();
        }

        public async Task<AppUser> GetAppuser(string userId) => await _context.Users
            .Where(u => u.Id == userId)
            .Include(u => u.JoinedEvents)
            .FirstOrDefaultAsync();

    }
}
