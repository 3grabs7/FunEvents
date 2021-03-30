using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FunEvents.Models
{
    public class ActiveUser : IdentityUser
    {
        public ICollection<Event> MyEvents { get; set; }
    }
}
