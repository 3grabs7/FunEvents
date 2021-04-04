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

namespace FunEvents.Pages.Events
{
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ActiveUser> _userManager;
        private readonly SignInManager<ActiveUser> _signInManager;

        public DetailsModel(ApplicationDbContext context,
           UserManager<ActiveUser> userManager,
           SignInManager<ActiveUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public ActiveUser ActiveUser { get; set; }
        public Event EventToJoin { get; set; }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            string userId = _userManager.GetUserId(User);
            ActiveUser = await _context.Users.Where(u => u.Id == userId).Include(u => u.MyEvents).FirstOrDefaultAsync();
            EventToJoin = await _context.Events.Where(e => e.Id == id).FirstOrDefaultAsync();
            ActiveUser.MyEvents.Add(EventToJoin);
            EventToJoin.SpotsAvailable--;
            await _context.SaveChangesAsync();

            return Page();
            //return RedirectToPage("./MyEvents");
        }
    }
}
