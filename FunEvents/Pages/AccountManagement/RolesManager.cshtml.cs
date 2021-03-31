using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FunEvents.Data;
using FunEvents.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FunEvents.Pages.AccountManagement
{
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
        public void OnGet(string userId, string roleName)
        {

        }
    }
}
