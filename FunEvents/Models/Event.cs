using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FunEvents.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Place { get; set; }
        public string Address { get; set; }
        public DateTime Date { get; set; }
        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }
        public int SpotsAvailable { get; set; }

        [InverseProperty("HostedEvents")]
        public AppUser Organizer { get; set; }

        [InverseProperty("JoinedEvents")]
        public ICollection<AppUser> Attendees { get; set; }
        public int PageVisits { get; set; }
        public int UniquePageVisits { get; set; }
        public ICollection<Event> EventChangesPendingManagerValidation { get; set; }
    }

    public class Analytics
    {
        public int Id { get; set; }
        public Event Event { get; set; }
        public string Ip { get; set; }
    }
}
