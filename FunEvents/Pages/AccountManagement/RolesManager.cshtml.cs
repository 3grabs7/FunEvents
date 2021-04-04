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

namespace FunEvents.Pages.AccountManagement
{
    [Authorize(Roles = "Admin")]
    public class RolesManagerModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ActiveUser> _userManager;

        public RolesManagerModel(ApplicationDbContext context,
            UserManager<ActiveUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task OnGetAsync()
        {
            Users = await _context.Users.ToListAsync();
        }

        public IList<ActiveUser> Users { get; set; }
        public ActiveUser ActiveUser { get; set; }
        // Only need this if want several roles to choose from
        //public IdentityRole Role { get; set; }
        // create OnPostADDASORGANIZERAsync method
        // create OnPostREMOVEASORGANIZERAsync method
        // shift between buttons with different form actions
        public async Task<IActionResult> OnPostAsync(string id)
        {
            // Get user from list
            ActiveUser = await _context.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            // Get role we would like to assign 
            // await _userManager.AddToRoleAsync(ActiveUser, Role.Name);
            // for now we only need organizer so if user is selected, orginizer role is assigned
            var result = await _userManager.AddToRoleAsync(ActiveUser, "Organizer");
            await _context.SaveChangesAsync();
            return RedirectToPage("/AccountManagement/Users");

        }

        public async Task<bool> IsOrganizer(string id)
        {
            IdentityRole role = await _context.Roles.Where(r => r.Name == "Organizer").FirstOrDefaultAsync();

            return await _context.UserRoles.Where(ur => ur.UserId == id && ur.RoleId == role.Id).FirstOrDefaultAsync() != default;
        }
    }
}
