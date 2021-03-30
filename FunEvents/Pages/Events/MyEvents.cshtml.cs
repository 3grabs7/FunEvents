using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FunEvents.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FunEvents.Models;

namespace FunEvents.Pages.Events
{
    public class MyEventsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public MyEventsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public ActiveUser ActiveUser { get; set; }

        public void OnGet()
        {
        }
    }
}
