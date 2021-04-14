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
    [Authorize(Roles = "OrganizationManager")]
    public class OrganizationRolesManagerModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public OrganizationRolesManagerModel(ApplicationDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<AppUser> Users { get; set; }
        public IList<IdentityRole> Roles { get; set; }

        // list of the current users organizations
        public IList<Organization> UserOrganizations { get; set; }

        public async Task OnGet()
        {
            Roles = await _context.Roles.ToListAsync();
            Users = await _context.Users.ToListAsync();

            var userId = _userManager.GetUserId(User);

            var currentUser = await _context.Users
                .Where(u => u.Id == userId)
                .Include(u => u.ManagerInOrganizations)
                .Include(u => u.AssistantInOrganizations)
                .FirstOrDefaultAsync();

            if (currentUser.ManagerInOrganizations?.Count != null)
            {
                UserOrganizations = currentUser.ManagerInOrganizations.ToList();
            }
        }

        public async Task<IActionResult> OnPostAddAsync(string id, string role, int organizationId)
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
                        return RedirectToPage("/AccountManagement/OrganizationRolesManager");
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
                
                await _context.SaveChangesAsync();

                return RedirectToPage("/AccountManagement/OrganizationRolesManager");
            }
            else if (role == "OrganizationAssistant")
            {
                Organization organization = await _context.Organizations.FindAsync(organizationId);

                selectedUser.AssistantInOrganizations.Add(organization);
            }

            await _userManager.AddToRoleAsync(selectedUser, role);
            await _context.SaveChangesAsync();

            if (_userManager.GetUserId(User) == selectedUser.Id)
            {
                await _userManager.UpdateSecurityStampAsync(selectedUser);
            }

            return RedirectToPage("/AccountManagement/OrganizationRolesManager");
        }

        public async Task<IActionResult> OnPostRemoveAsync(string id, string role, int organizationId)
        {
            var user = await SelectedUser(id);
            var result = await _userManager.RemoveFromRoleAsync(user, role);

            Organization organization = await _context.Organizations.FindAsync(organizationId);
            
            if(role == "OrganizationManager")
            {
                user.ManagerInOrganizations.Remove(organization);
            }
            else if (role == "OrganizationAssistant")
            {
                user.AssistantInOrganizations.Remove(organization);
            }

            if (!result.Succeeded)
            {
                throw new Exception(String.Join(' ', result.Errors.Select(e => e.Description)));
            }

            await _context.SaveChangesAsync();

            return RedirectToPage("/AccountManagement/OrganizationRolesManager");
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

        // to check if user is manager for specific organization
        public async Task<bool> IsManagerFor(int organizationId, string userId)
        {
            AppUser user = await SelectedUser(userId);

            var organizationsWhereUserIsManager = user.ManagerInOrganizations.ToList();
            foreach (var organization in organizationsWhereUserIsManager)
            {
                if (organization.Id == organizationId)
                {
                    return true;
                }
            }
            return false;
        }

        // to check if user is manager for specific organization
        public async Task<bool> IsAssistantFor(int organizationId, string userId)
        {
            AppUser user = await SelectedUser(userId);

            var organizationsWhereUserIsAssistant = user.AssistantInOrganizations.ToList();
            foreach (var organization in organizationsWhereUserIsAssistant)
            {
                if (organization.Id == organizationId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
