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
        public ActiveUser ActiveUser { get; set; }
        public IList<Event> Events { get; set; }
        public async Task<IActionResult> OnGet()
        {
            string userId = _userManager.GetUserId(User);

            Events = await _context.Events.Where(e => e.Organizer.ActiveUser.Id == userId).ToListAsync();

            return Page();
        }
    }
}
