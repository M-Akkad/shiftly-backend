using DAL;
using DAL.Models;

namespace Shiftly.Data;

public static class DbInitializer
{
    public static void Initialize(ShiftlyContext context)
    {
        // Database aanmaken
        context.Database.EnsureCreated();

        // Check of er al data is
        if (context.Admins.Any())
        {
            return; // Database is al geseeded
        }

        // Seed Admins
        var admins = new Admin[]
        {
            new Admin
            {
                Naam = "Jan de Trainer",
                Email = "jan@shiftly.nl",
                WachtwoordHash = "tempwachtwoord123" // TODO: Hash implementeren
            },
            new Admin
            {
                Naam = "Piet Beheerder",
                Email = "piet@shiftly.nl",
                WachtwoordHash = "tempwachtwoord123"
            }
        };

        context.Admins.AddRange(admins);
        context.SaveChanges();

        // Seed Spelers
        var spelers = new Speler[]
        {
            new Speler { Naam = "Mohammed Ali", Email = "m.ali@team.nl", Positie = "Aanvaller", Rugnummer = 9, WachtwoordHash = "speler123" },
            new Speler { Naam = "Sara de Vries", Email = "s.devries@team.nl", Positie = "Middenvelder", Rugnummer = 10, WachtwoordHash = "speler123" },
            new Speler { Naam = "Ahmed Hassan", Email = "a.hassan@team.nl", Positie = "Verdediger", Rugnummer = 5, WachtwoordHash = "speler123" },
            new Speler { Naam = "Lisa Jansen", Email = "l.jansen@team.nl", Positie = "Keeper", Rugnummer = 1, WachtwoordHash = "speler123" },
            new Speler { Naam = "Kevin Bakker", Email = "k.bakker@team.nl", Positie = "Aanvaller", Rugnummer = 11, WachtwoordHash = "speler123" },
            new Speler { Naam = "Fatima El Amrani", Email = "f.elamrani@team.nl", Positie = "Middenvelder", Rugnummer = 8, WachtwoordHash = "speler123" },
            new Speler { Naam = "Thomas Visser", Email = "t.visser@team.nl", Positie = "Verdediger", Rugnummer = 3, WachtwoordHash = "speler123" },
            new Speler { Naam = "Yasmin Özdemir", Email = "y.ozdemir@team.nl", Positie = "Aanvaller", Rugnummer = 7, WachtwoordHash = "speler123" }
        };

        context.Spelers.AddRange(spelers);
        context.SaveChanges();

        // Seed Wedstrijden (in de toekomst, zodat ze bruikbaar zijn voor testen)
        var wedstrijden = new Wedstrijd[]
        {
            new Wedstrijd
            {
                Datum = DateTime.Now.AddDays(10).Date,
                Tijd = new TimeSpan(14, 0, 0), // 14:00
                Locatie = "Sportpark De Toekomst",
                Tegenstander = "FC Utrecht",
                AdminID = admins[0].ID
            },
            new Wedstrijd
            {
                Datum = DateTime.Now.AddDays(17).Date,
                Tijd = new TimeSpan(16, 30, 0), // 16:30
                Locatie = "Cruyff Arena",
                Tegenstander = "PSV",
                AdminID = admins[0].ID
            },
            new Wedstrijd
            {
                Datum = DateTime.Now.AddDays(24).Date,
                Tijd = new TimeSpan(13, 0, 0), // 13:00
                Locatie = "Sportcomplex Zuid",
                Tegenstander = "Feyenoord",
                AdminID = admins[1].ID
            }
        };

        context.Wedstrijden.AddRange(wedstrijden);
        context.SaveChanges();

        // Seed WedstrijdSpelers (wijs enkele spelers toe aan wedstrijden)
        var wedstrijdSpelers = new WedstrijdSpeler[]
        {
            // Wedstrijd 1
            new WedstrijdSpeler { WedstrijdID = wedstrijden[0].ID, SpelerID = spelers[0].ID, Status = "Aanwezig" },
            new WedstrijdSpeler { WedstrijdID = wedstrijden[0].ID, SpelerID = spelers[1].ID, Status = "Aanwezig" },
            new WedstrijdSpeler { WedstrijdID = wedstrijden[0].ID, SpelerID = spelers[2].ID, Status = "Aanwezig" },
            new WedstrijdSpeler { WedstrijdID = wedstrijden[0].ID, SpelerID = spelers[3].ID, Status = "Aanwezig" },

            // Wedstrijd 2
            new WedstrijdSpeler { WedstrijdID = wedstrijden[1].ID, SpelerID = spelers[4].ID, Status = "Aanwezig" },
            new WedstrijdSpeler { WedstrijdID = wedstrijden[1].ID, SpelerID = spelers[5].ID, Status = "Afwezig" },
            new WedstrijdSpeler { WedstrijdID = wedstrijden[1].ID, SpelerID = spelers[6].ID, Status = "Aanwezig" },

            // Wedstrijd 3
            new WedstrijdSpeler { WedstrijdID = wedstrijden[2].ID, SpelerID = spelers[0].ID, Status = "Aanwezig" },
            new WedstrijdSpeler { WedstrijdID = wedstrijden[2].ID, SpelerID = spelers[7].ID, Status = "Aanwezig" }
        };

        context.WedstrijdSpelers.AddRange(wedstrijdSpelers);
        context.SaveChanges();

        Console.WriteLine("✓ Database succesvol geseeded met test data!");
    }
}
