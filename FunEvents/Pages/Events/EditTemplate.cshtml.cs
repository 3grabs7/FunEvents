﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FunEvents.Data;
using FunEvents.Models;
using Microsoft.AspNetCore.Identity;

namespace FunEvents.Pages.Events
{
    public class EditTemplateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public EditTemplateModel(ApplicationDbContext context,
             UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public IList<Event> Events { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            string userId = _userManager.GetUserId(User);

            Events = await _context.Events.Where(e => e.Organizer.Id == userId).ToListAsync();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            bool wasUpdateSuccessfull;
            foreach (Event updatedEvents in Events)
            {
                wasUpdateSuccessfull = await TryUpdateModelAsync<Event>(updatedEvents, "event",
                e => e.Title, e => e.Description, e => e.Date, e => e.Place, e => e.Address, e => e.SpotsAvailable);

                if (!wasUpdateSuccessfull)
                {
                    return Page();
                }
            }

            _context.AttachRange(Events);

            return RedirectToPage("./EditTemplate");
        }

    }
}
