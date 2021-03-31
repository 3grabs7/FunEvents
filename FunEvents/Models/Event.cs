using System;
using System.Collections.Generic;
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
        public int SpotsAvailable { get; set; }

        public List<ActiveUser> Attendees { get; set; }
        public Organizer Organizer { get; set; }

    }

    // Tillfällig klass
    public class Organizer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ActiveUser ActiveUser { get; set; }
        public List<Event> Events { get; set; }
    }
}
