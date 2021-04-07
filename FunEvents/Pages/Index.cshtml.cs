using FunEvents.Data;
using FunEvents.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace FunEvents.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public IndexModel(ILogger<IndexModel> logger,
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task OnGetAsync(bool? seedDb)
        {
            if (seedDb ?? false)
            {
                await _context.SeedDatabase(_userManager);
            }
        }

        private const int EVENTS_PER_TOP_VIEW = 3;
        public async Task<IList<Event>> LoadPopularEvents()
        {
            var events = await _context.Events
                .Where(e => e.SpotsAvailable > 0)
                .OrderBy(e => e.PageVisits)
                .Reverse()
                .Take(EVENTS_PER_TOP_VIEW)
                .ToListAsync();
            return events;
        }

        public async Task<IList<Event>> LoadNewEvents()
        {
            var events = await _context.Events
                .Where(e => e.SpotsAvailable > 0)
                .OrderBy(e => e.CreatedAt)
                .Reverse()
                .Take(EVENTS_PER_TOP_VIEW)
                .ToListAsync();
            return events;
        }

        public async Task<IList<Event>> LoadAlmostFullyBookedEvents()
        {
            var events = await _context.Events
                .Where(e => e.SpotsAvailable > 0)
                .OrderBy(e => e.SpotsAvailable)
                .Take(EVENTS_PER_TOP_VIEW)
                .ToListAsync();
            return events;
        }
    }
}
