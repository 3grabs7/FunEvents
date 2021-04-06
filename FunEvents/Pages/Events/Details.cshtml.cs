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
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public DetailsModel(ApplicationDbContext context,
           UserManager<AppUser> userManager,
           SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public Event EventToJoin { get; set; }
        public AppUser AppUser { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                // Add Custom Page for problem loading event from db
                return NotFound();
            }

            EventToJoin = await _context.Events.FindAsync(id);
            string userId = _userManager.GetUserId(User);
            AppUser = await _context.Users.Where(u => u.Id == userId).Include(u => u.JoinedEvents).FirstOrDefaultAsync();

            if (AppUser == default)
            {
                // Add Custom Page for problem with finding user, log in log out
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                // Add Custom Page for problem loading event from db
                return NotFound();
            }

            EventToJoin = await _context.Events.FindAsync(id);

            string userId = _userManager.GetUserId(User);
            AppUser appUser = await _context.Users.Where(u => u.Id == userId).Include(u => u.JoinedEvents).FirstOrDefaultAsync();

            try
            {
                appUser.JoinedEvents.Add(EventToJoin);
                EventToJoin.SpotsAvailable--;

                await _context.SaveChangesAsync();

            }
            catch (Exception e)
            {
                // Add Custom Page for problem joining event
                // Route Exception
                return Page();
            }

            return RedirectToPage("/Events/Details", new { id = id });
        }

        public int AttendeesCount() => _context.Events
            .Include(e => e.Attendees)
            .Where(e => e.Id == EventToJoin.Id)
            ?.First().Attendees.Count
            ?? 0;

        public string GetAttendeeInfo()
        {
            var attendees = _context.Events
                .Include(e => e.Attendees)
                .Where(e => e.Id == EventToJoin.Id)
                .First().Attendees;

            string output = String.Join('\n', attendees?.Select(a => a.UserName)
                ?? new List<string> { "Couldn't Load Users" });

            return output;
        }


    }
}
