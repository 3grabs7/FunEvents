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
        public ActiveUser SelectedUser { get; set; }
        // Only need this if we want several roles to choose from
        //public IdentityRole Role { get; set; }
        public async Task<IActionResult> OnPostAddAsync(string id)
        {
            SelectedUser = await _context.Users
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
            // await _userManager.AddToRoleAsync(ActiveUser, Role.Name);
            await _userManager.AddToRoleAsync(SelectedUser, "Organizer");
            await _context.SaveChangesAsync();

            if (_userManager.GetUserId(User) == SelectedUser.Id)
            {
                await _userManager.UpdateSecurityStampAsync(SelectedUser);
            }

            return RedirectToPage("/AccountManagement/RolesManager");
        }

        public async Task<IActionResult> OnPostRemoveAsync(string id)
        {
            SelectedUser = await _context.Users
                .Where(u => u.Id == id)
                .FirstOrDefaultAsync();
            var result = await _userManager.RemoveFromRoleAsync(SelectedUser, "Organizer");
            await _context.SaveChangesAsync();

            return RedirectToPage("/AccountManagement/RolesManager");
        }

        public async Task<bool> IsOrganizer(string id)
        {
            IdentityRole role = await _context.Roles
                .Where(r => r.Name == "Organizer")
                .FirstOrDefaultAsync();

            return await _context.UserRoles
                .AnyAsync(ur => ur.UserId == id && ur.RoleId == role.Id);
        }
    }
}
