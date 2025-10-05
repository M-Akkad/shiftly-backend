namespace Shiftly.Models;

public class Wedstrijd
{
    public int Id { get; set; }
    public DateTime Datum { get; set; }   // bevat datum + tijd
    public string Locatie { get; set; } = "";
    public string Tegenstander { get; set; } = "";
    public ICollection<WedstrijdSpeler> WedstrijdSpelers { get; set; } = new List<WedstrijdSpeler>();
}
