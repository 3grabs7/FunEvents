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
            Roles = await _context.Roles.ToListAsync();
            Users = await _context.Users.ToListAsync();
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

        // Will merge with other add/remove posts once completely tested
        public async Task<IActionResult> OnPostCreateOrganizerAsync(string id)
        {
            AppUser user = await SelectedUser(id);

            if (user.ManagerInOrganizations?.Count > 0)
            {
                // make sure that user is not already manager for an unverified organization
                bool hasUnverifiedOrganization = user.ManagerInOrganizations
                    .Any(o => !o.IsVerified);
                if (hasUnverifiedOrganization)
                {
                    // once tested, make sure this is checked before loading list
                    // and button for adding user as organizationmanager is disabled
                    Console.WriteLine("ALREADY PENDING VALIDATION");
                    return RedirectToPage("/AccountManagement/RolesManager");
                }
            }

            await _userManager.AddToRoleAsync(user, "OrganizerManager");
            Organizer organizer = new Organizer()
            {
                Name = "Unverified",
                IsVerified = false,
                OrganizerManagers = new List<AppUser>() { }
            };
            await _context.Organizers.AddAsync(organizer);
            user.ManagerInOrganizations.Add(organizer);
            await _context.SaveChangesAsync();

            return RedirectToPage("/AccountManagement/RolesManager");
        }

        public async Task<AppUser> SelectedUser(string id) => await _context.Users
            .Where(u => u.Id == id)
            .Include(u => u.ManagerInOrganizations)
            .Include(u => u.AssistantInOrganizations)
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
