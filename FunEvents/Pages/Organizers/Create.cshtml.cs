using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using FunEvents.Data;
using FunEvents.Models;

namespace FunEvents.Pages.Organizers
{
    public class CreateModel : PageModel
    {
        private readonly FunEvents.Data.ApplicationDbContext _context;

        public CreateModel(FunEvents.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Organization Organizer { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Organizations.Add(Organizer);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
