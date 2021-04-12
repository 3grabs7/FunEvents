using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FunEvents.Data;
using FunEvents.Models;

namespace FunEvents.Pages.Organizers
{
    public class DeleteModel : PageModel
    {
        private readonly FunEvents.Data.ApplicationDbContext _context;

        public DeleteModel(FunEvents.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Organizer Organizer { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Organizer = await _context.Organizers.FirstOrDefaultAsync(m => m.Id == id);

            if (Organizer == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Organizer = await _context.Organizers.FindAsync(id);

            if (Organizer != null)
            {
                _context.Organizers.Remove(Organizer);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
