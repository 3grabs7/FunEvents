using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FunEvents.Data;
using FunEvents.Models;

namespace FunEvents.Pages.Organizers
{
    public class EditModel : PageModel
    {
        private readonly FunEvents.Data.ApplicationDbContext _context;

        public EditModel(FunEvents.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Organization Organizer { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Organizer = await _context.Organizations.FirstOrDefaultAsync(m => m.Id == id);

            if (Organizer == null)
            {
                return NotFound();
            }
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Organizer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrganizerExists(Organizer.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool OrganizerExists(int id)
        {
            return _context.Organizations.Any(e => e.Id == id);
        }
    }
}
