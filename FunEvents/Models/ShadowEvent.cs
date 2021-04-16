using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FunEvents.Models
{
    public class ShadowEvent : EditableEvent
    {
        public AppUser Editor { get; set; }
        public Event PendingEditEvent { get; set; }

    }
}
