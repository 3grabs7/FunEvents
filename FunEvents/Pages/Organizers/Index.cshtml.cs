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
    public class IndexModel : PageModel
    {
        private readonly FunEvents.Data.ApplicationDbContext _context;

        public IndexModel(FunEvents.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Organization> Organizer { get;set; }

        public async Task OnGetAsync()
        {
            Organizer = await _context.Organizations.ToListAsync();
        }
    }
}
