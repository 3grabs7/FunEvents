using System;
using FunEvents.Models;
using FunEvents.Data;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System.Text.RegularExpressions;

namespace FunEvents.Pages.Events
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;

        public IndexModel(ApplicationDbContext context,
            UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SortBy { get; set; }

        public int Count { get; set; }
        public int PageSize { get; set; } = 9;
        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));
        public IList<Event> Events { get; set; }

        public async Task OnGetAsync()
        {
            if (String.IsNullOrWhiteSpace(SortBy))
            {
                Events = await GetPaginatedResult(CurrentPage, PageSize);
            }
            else
            {
                Events = await GetPaginatedResult(CurrentPage, PageSize, SortBy);
            }

            Count = await GetCount();
        }

        public async Task<AppUser> GetAppUser(string userId) => await _context.Users
            .Where(u => u.Id == userId)
            .Include(u => u.JoinedEvents)
            .FirstOrDefaultAsync();

        public async Task<List<Event>> GetPaginatedResult(int currentPage, int pageSize)
        {
            IList<Event> data = await _context.Events.ToListAsync();
            return data.OrderBy(d => d?.CreatedAt)
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

            if (sortResultsBy == "Date" ||
                sortResultsBy == "SpotsAvailable")
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

        public string CondenseDescription(string input)
        {
            if (input == null) return "No description, weird huh?";
            string[] splitInput = Regex.Split(input, @"[!?.]");
            return splitInput.Length == 1 ?
                $"{splitInput[0]}." :
                $"{splitInput[0]}...";
        }

    }
}
