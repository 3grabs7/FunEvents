using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FunEvents.Models
{
    public class Analytics
    {
        public int Id { get; set; }
        public Event Event { get; set; }
        public string Ip { get; set; }
    }
}
