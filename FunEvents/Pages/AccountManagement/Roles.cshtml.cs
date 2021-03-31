using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FunEvents.Data;
using FunEvents.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FunEvents.Pages.AccountManagement
{
    public class RolesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ActiveUser> _userManager;

        public RolesModel(ApplicationDbContext context,
            UserManager<ActiveUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public IList<IdentityRole> Roles { get; set; }
        [BindProperty]
        public IdentityRole NewRole { get; set; }
        public void OnGetAsync()
        {
            Roles = _context.Roles.ToList();
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            NewRole.NormalizedName = NewRole.Name.ToUpper();
            await _context.Roles.AddAsync(NewRole);
            await _context.SaveChangesAsync();
            return RedirectToPage("/AccountManagement/Roles");
        }

        public async Task<IActionResult> OnPostRemoveAsync(string id)
        {
            IdentityRole roleToRemove = await _context.Roles.Where(r => r.Id == id).FirstOrDefaultAsync();
            _context.Roles.Remove(roleToRemove);
            await _context.SaveChangesAsync();
            return RedirectToPage("/AccountManagement/Roles");
        }
    }
}
