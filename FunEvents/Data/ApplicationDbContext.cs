using FunEvents.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FunEvents.Data
{
    public class ApplicationDbContext : IdentityDbContext<ActiveUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Organizer> Organizers { get; set; }

        public async Task SeedDatabase(UserManager<ActiveUser> userManager)
        {
            await Database.EnsureDeletedAsync();
            await Database.EnsureCreatedAsync();

            await Events.AddRangeAsync(new List<Event>() {
                new Event{Title="Food Festival", Description="All you can eat - food from all around the world - all you need is a ticket!", Place="On the street", Address="Gourmet Lane 63", Date= new DateTime(2021,7,21), SpotsAvailable=1000},
                new Event{Title="Free Karaoke night",  Description="Join this event to be a part of a fantastic karaoke night!", Place="Sing Along", Address="Vocals street 2", Date= new DateTime(2021,5,13), SpotsAvailable=35 },
                new Event{Title="Run for fun",  Description="A race with no competition, just for fun! Get amazing treats on the way!", Place="In the woods", Address="Green forest 23", Date= new DateTime(2021,6,5), SpotsAvailable=500 },
                new Event{Title="Poetry slam",  Description="Got a poem? Wanna perform? Join this event to enter the competition, or be the audience!", Place="The Green Lion", Address="Lonely road 4", Date= new DateTime(2021,4,30), SpotsAvailable=65 },
                new Event{Title="Finger Painting", Description="Bring your inner child and join this event to get creative!", Place="Art factory", Address="Artsy road 31", Date= new DateTime(2021,10,11), SpotsAvailable=30 },
                new Event{Title="Splash away",  Description="Exclusive offer for the Shark Water Park premiere! 2 for 1 entrance", Place="Shark Water park", Address="Ocean lane 11", Date= new DateTime(2021,5,21), SpotsAvailable=650 },
                new Event{Title="Heroes 3 LAN", Description="Nybörjarvänlig spelkväll med the one and only Björn. " +
            "Vare sig du har spelat sen barnsben eller skådar detta överkomplicerade mästerverk för första gången kommer det gå lika dåligt. " +
            "Alla är n00bs i en lobby med Björn. Du kommer gråta, men, vi kommer gråta tillsammans. Alla utom Björn.",
                Place="Other", Address="Riktiga Gatan 1, 420 69 Göteborg", Date=DateTime.Now, SpotsAvailable=5},

            new Event{Title="Skriftlig Tentamen i Astrofysik", Description="Har du listat ut hur interfaces fungerar i c#? " +
            "Kan du stava till och konstruktivt använda polymorphism? Grattis! För isåfall är en månlandning inte svårare en att hata css.",
                Place="Other", Address="Riktiga Gatan 7, 420 69 Göteborg", Date=DateTime.Now, SpotsAvailable=42},

            new Event{Title="Frågestund med Björn", Description="Är ditt huvud fyllt med kodrelaterade frågor och " +
            "problematiseringar? Är din hjärnas allokerade plats för svar mindre en din chans till en LIAplats i höst? Hoppa in i frågestund med Björn, " +
            "bevittna din fråga hänga död i luften, se på när han misslyckas med multitasking och hör hur han fnissar åt något roligt någon annan, " +
            "mycket mer begåvad elev, skrev i ett dm på discord. Det blir kul!",
                Place="Other", Address="Microsoft Teams -12, 420 69 Göteborg", Date=DateTime.Now, SpotsAvailable=404},

            new Event{Title="Fest. ?", Description="Man blir sugen eller hur?", Place="Other",
                Address="Tralala 6, 420 69 Günther", Date=DateTime.Now, SpotsAvailable=14},

            new Event{Title="Livet Under Knät", Description="För alla er som njuter av  dödslängtande suckar och hatar " +
            "brusreducering i mikrofoner. Här kommer en gala för tvetydighet, ett berg av överkvalifikation och en hel stad av beklämmelse.",
                Place="Other", Address="Jordens Kant 11, 420 69 Havet", Date=DateTime.Now, SpotsAvailable=999},

            new Event{Title="Kalldusch med Anna Book", Description="Snacka tänder, skönhetsingrepp och förutfattande " +
            "meningar om ryssar. Face-to-face med Sveriges knepigaste leende.",
                Place="Other", Address="Nånstanns I Stan, 420 69 Stockholm", Date=DateTime.Now, SpotsAvailable=1},

            new Event{Title="Nopp", Description="Det blir inget med det.", Place="Other",
                Address="Alcingsåös, 420 69 ½Göteborg", Date=DateTime.Now, SpotsAvailable=0},

            new Event{Title="Gymma Med Jack (bara biceps)", Description="...", Place="Other",
                Address="Arnoldstråket 198, 420 69 Göteborg", Date=DateTime.Now, SpotsAvailable=23},

            new Event{Title="Bootflak", Description="🔞🔞🔞🔞🔞🔞🔞🔞", Place="Other",
                Address="Riktiga Gatan 0, 420 69 Göteborg", Date=DateTime.Now, SpotsAvailable=2},

            new Event{Title="Abc", Description="Fest o grej, o kul, och ja. Gör allt, lite till, mer " +
            "Visst vettu, åk åk åk gogogogogogo", Place="Other",
                Address="Riktiga Gatan 0, 420 69 Göteborg", Date=DateTime.Now, SpotsAvailable=8},

            new Event{Title="Röj Va", Description="Visst vettu, åk åk åk gogogogogogo", Place="Other",
                Address="Riktiga Gatan 0, 420 69 Göteborg", Date=DateTime.Now, SpotsAvailable=40},

            new Event{Title="Fuck You",  Description="Visst vettu, åk åk åk gogogogogogo", Place="Other",
                Address="Riktiga Gatan 0, 420 69 Göteborg", Date=DateTime.Now, SpotsAvailable=370},

            new Event{Title="Plz plz",  Description="Visst vettu, åk åk åk gogogogogogo", Place="Other",
                Address="Riktiga Gatan 0, 420 69 Göteborg", Date=DateTime.Now, SpotsAvailable=37},

            new Event{Title="Sjukaste Eventet",  Description="Visst vettu, åk åk åk gogogogogogo", Place="Other",
                Address="Riktiga Gatan 0, 420 69 Göteborg", Date=DateTime.Now, SpotsAvailable=372},

            new Event{Title="Knappt Kul", Description="Visst vettu, åk åk åk gogogogogogo", Place="Other",
                Address="Riktiga Gatan 0, 420 69 Göteborg", Date=DateTime.Now, SpotsAvailable=337},

            new Event{Title="Eller annars då",  Description="Visst vettu, åk åk åk gogogogogogo", Place="Other",
                Address="Riktiga Gatan 0, 420 69 Göteborg", Date=DateTime.Now, SpotsAvailable=0},

            });

            await SaveChangesAsync();
        }

    }
}
