using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FunEvents.Data;
using FunEvents.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FunEvents.Pages.AccountManagement
{
    [Authorize(Roles = "Admin")]
    public class ManageRolesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public ManageRolesModel(ApplicationDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public AppUser AppUser { get; set; }
        public IList<AppUser> Users { get; set; }

        public async Task<IActionResult> OnGet()
        {
            string userId = _userManager.GetUserId(User);

            AppUser = await _context.Users.Where(u => u.Id == userId).SingleOrDefaultAsync();
            Users = await _context.Users.ToListAsync();

            return Page();
        }

    }
}
