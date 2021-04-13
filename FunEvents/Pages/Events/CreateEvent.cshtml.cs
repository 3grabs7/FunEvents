using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FunEvents.Data;
using FunEvents.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FunEvents.Pages.Events
{
    [Authorize(Roles = "Admin, OrganizerManager")]
    public class CreateEventModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;

        public CreateEventModel(ApplicationDbContext context,
           UserManager<AppUser> userManager,
           SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [BindProperty]
        public Event NewEvent { get; set; }
        [BindProperty]
        public Organizer Organizer { get; set; }
        public ICollection<Organizer> OrganizersWhereUserIsManager { get; set; }

        public async Task OnGetAsync()
        {
            var user = await GetAppUser(_userManager.GetUserId(User));
            OrganizersWhereUserIsManager = await _context.Organizers
                .Where(o => o.OrganizerManagers.Contains(user))
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            NewEvent.Organizer = await _context.Organizers
                .FindAsync(Convert.ToInt32(Request.Form["organizer"]));
            NewEvent.CreatedAt = DateTime.Now;
            await _context.Events.AddAsync(NewEvent);

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        public async Task<AppUser> GetAppUser(string userId) => await _context.Users
            .Where(u => u.Id == userId)
            .Include(u => u.ManagerInOrganizations)
            .FirstOrDefaultAsync();
    }
}
