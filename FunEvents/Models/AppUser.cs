using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FunEvents.Models
{
    public class AppUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<Event> JoinedEvents { get; set; }

        public ICollection<Organizer> ManagerInOrganizations { get; set; }

        public ICollection<Organizer> AssistantInOrganizations { get; set; }
    }

}
