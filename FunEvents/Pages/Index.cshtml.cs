using FunEvents.Data;
using FunEvents.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FunEvents.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private const int EVENTS_PER_TOP_VIEW = 3;

        [BindProperty]
        public Organization OrganizationToBeValidated { get; set; }

        public IndexModel(ILogger<IndexModel> logger,
            ApplicationDbContext context,
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> OnGetAsync(bool? seedDb)
        {
            // "Reset Db" will redirect us to Index and route seedDb
            if (seedDb ?? false)
            {
                await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
                await _context.SeedDatabase(_userManager, _roleManager);
            }

            if (IsOrganizerPendingVerification().Result)
            {
                var user = await GetAppuser(_userManager.GetUserId(User));
                OrganizationToBeValidated = user.ManagerInOrganizations
                    .First(o => !o.IsVerified);
            }

            return Page();
        }

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

        public async Task<AppUser> GetAppuser(string userId) => await _context.Users
            .Where(u => u.Id == userId)
            .Include(u => u.ManagerInOrganizations)
            .Include(u => u.AssistantInOrganizations)
            .FirstOrDefaultAsync();

        public async Task<bool> IsOrganizerPendingVerification()
        {
            // If user is anonymous or lack "OrganizerManager role, return immediately
            if (!User.Identity.IsAuthenticated) return false;
            if (!User.IsInRole("OrganizationManager")) return false;

            var user = await GetAppuser(_userManager.GetUserId(User));
            var managerForUnverifiedOrganizer = user.ManagerInOrganizations
                .Any(o => !o.IsVerified);
            if (managerForUnverifiedOrganizer)
            {
                // If unverified organizer is found, set our binded property before returning
                OrganizationToBeValidated = await GetUnverifiedOrganization();
                return true;
            }
            return false;
        }

        public async Task<Organization> GetUnverifiedOrganization()
        {
            var user = await GetAppuser(_userManager.GetUserId(User));
            return user.ManagerInOrganizations
                .First(o => !o.IsVerified);
        }

        public async Task<IActionResult> OnPostVerifyOrganizationAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            OrganizationToBeValidated.IsVerified = true;
            _context.Attach(OrganizationToBeValidated).State = EntityState.Modified;

            var currentUser = await GetAppuser(_userManager.GetUserId(User));

            // making current user manager of the created organization
            currentUser.ManagerInOrganizations.Add(OrganizationToBeValidated);

            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
    }
}
