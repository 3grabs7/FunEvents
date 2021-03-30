using System;
using FunEvents.Models;
using FunEvents.Data;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FunEvents.Pages.Events
{
    public class IndexModel : PageModel
    {

        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; }
        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; }
        public int Count { get; set; }
        public int PageSize { get; set; } = 5;

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

        public IList<Event> Event { get; set; }

        public async Task OnGetAsync()
        {
            if (String.IsNullOrWhiteSpace(SortBy))
            {
                Event = await GetPaginatedResult(CurrentPage, PageSize);
            }
            else
            {
                Event = await GetPaginatedResult(CurrentPage, PageSize, SortBy);
            }
            Count = await GetCount();
        }

        public async Task<List<Event>> GetPaginatedResult(int currentPage, int pageSize)
        {
            IList<Event> data = await _context.Events.ToListAsync();
            return data.OrderBy(d => d.Id)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToList();
        }

        public async Task<List<Event>> GetPaginatedResult(int currentPage, int pageSize, string sortResultsBy)
        {
            List<Event> data = await _context.Events.AsQueryable()
                .OrderBy(sortResultsBy)
                .Skip((currentPage - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (sortResultsBy != "Title")
            {
                data.Reverse();
            }
            return data;

        }

        public async Task<int> GetCount()
        {
            List<Event> data = await _context.Events.ToListAsync();
            return data.Count;
        }
    }
}
