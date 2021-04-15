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
    [Authorize(Roles = "Admin")]
    public class AdminRolesManagerModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminRolesManagerModel(ApplicationDbContext context,
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
            AppUser selectedUser = await SelectedUser(id);

            if (role == "OrganizationManager")
            {
                if (selectedUser.ManagerInOrganizations?.Count > 0)
                {
                    // make sure that user is not already manager for an unverified organization
                    bool hasUnverifiedOrganization = selectedUser.ManagerInOrganizations
                        .Any(o => !o.IsVerified);
                    if (hasUnverifiedOrganization)
                    {
                        // once tested, make sure this is checked before loading list
                        // and button for adding user as organizationmanager is disabled
                        Console.WriteLine("ALREADY PENDING VALIDATION");
                        return RedirectToPage("/AccountManagement/AdminRolesManager");
                    }
                }

                await _userManager.AddToRoleAsync(selectedUser, "OrganizationManager");
                Organization organization = new Organization()
                {
                    Name = "Unverified",
                    IsVerified = false,
                    OrganizationManagers = new List<AppUser>() { }
                };
                await _context.Organizations.AddAsync(organization);
                selectedUser.ManagerInOrganizations.Add(organization);
                await _context.SaveChangesAsync();

                return RedirectToPage("/AccountManagement/AdminRolesManager");
            }

            await _userManager.AddToRoleAsync(selectedUser, role);
            await _context.SaveChangesAsync();

            if (_userManager.GetUserId(User) == selectedUser.Id)
            {
                await _userManager.UpdateSecurityStampAsync(selectedUser);
            }

            return RedirectToPage("/AccountManagement/AdminRolesManager");
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

            return RedirectToPage("/AccountManagement/AdminRolesManager");
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
