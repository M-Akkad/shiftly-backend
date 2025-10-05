namespace Shiftly.Models;

public enum AanwezigheidStatus { Onbekend = 0, Aanwezig = 1, Afwezig = 2 }

public class WedstrijdSpeler
{
    public int Id { get; set; }
    public int WedstrijdId { get; set; }
    public Wedstrijd? Wedstrijd { get; set; }

    public string SpelerId { get; set; } = "";
    public ApplicationUser? Speler { get; set; }

    public string Rol { get; set; } = "";
    public AanwezigheidStatus Status { get; set; } = AanwezigheidStatus.Onbekend;
}

