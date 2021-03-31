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
    //[Authorize(Roles = "Administrator")]
    public class ManageRolesModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ActiveUser> _userManager;

        public ManageRolesModel(ApplicationDbContext context, UserManager<ActiveUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public ActiveUser ActiveUser { get; set; }
        public IList<ActiveUser> Users { get; set; }

        public async Task<IActionResult> OnGet()
        {
            string userId = _userManager.GetUserId(User);

            ActiveUser = await _context.Users.Where(u => u.Id == userId).SingleOrDefaultAsync();
            Users = await _context.Users.ToListAsync();

            return Page();
        }
    }
}
