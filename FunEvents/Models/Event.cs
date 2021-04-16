using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FunEvents.Models
{
    public class Event : EditableEvent
    {
        public Organization Organization { get; set; }

        [InverseProperty("JoinedEvents")]
        public ICollection<AppUser> Attendees { get; set; }
        public ICollection<ShadowEvent> EventChangesPendingManagerValidation { get; set; }

        [Display(Name = "Created")]
        public DateTime CreatedAt { get; set; }
        public int PageVisits { get; set; }
        public int UniquePageVisits { get; set; }
    }

}
