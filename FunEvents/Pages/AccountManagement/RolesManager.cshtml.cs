using FunEvents.Data;
using FunEvents.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FunEvents.Pages.AccountManagement
{
    [Authorize(Roles = "Admin, Organizer")]
    public class RolesManagerModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public RolesManagerModel(ApplicationDbContext context,
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IList<AppUser> Users { get; set; }

        public IList<IdentityRole> Roles { get; set; }

        public async Task OnGetAsync()
        {
            Users = await _context.Users.ToListAsync();
        }

        private string[] GetRolesForUser(ClaimsPrincipal user)
        {
            throw new System.NotImplementedException();
        }

        public async Task<IActionResult> OnPostAddAsync(string id, string role)
        {
            var user = await SelectedUser(id);
            await _userManager.AddToRoleAsync(user, role);
            await _context.SaveChangesAsync();

            if (_userManager.GetUserId(User) == user.Id)
            {
                await _userManager.UpdateSecurityStampAsync(user);
            }

            return RedirectToPage("/AccountManagement/RolesManager");
        }

        public async Task<IActionResult> OnPostRemoveAsync(string id, string role)
        {
            var user = await SelectedUser(id);
            var result = await _userManager.RemoveFromRoleAsync(user, role);

            if (!result.Succeeded)
            {
                throw new Exception(String.Join(' ', result.Errors.Select(e => e.Description)));
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/AccountManagement/RolesManager");
        }

        public async Task<AppUser> SelectedUser(string id) => await _context.Users
            .Where(u => u.Id == id)
            .FirstOrDefaultAsync();

        public async Task<bool> IsInSpecificRole(string id, string role)
        {
            IdentityRole roleToCheck = await _context.Roles
                .Where(r => r.Name == role)
                .FirstOrDefaultAsync();

            return await _context.UserRoles
                .AnyAsync(ur => ur.UserId == id && ur.RoleId == roleToCheck.Id);
        }

    }
}
