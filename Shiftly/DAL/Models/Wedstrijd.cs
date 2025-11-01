namespace DAL.Models;

public class Wedstrijd
{
    public int ID { get; set; }
    public DateTime Datum { get; set; }
    public TimeSpan Tijd { get; set; }
    public string Locatie { get; set; } = string.Empty;
    public string Tegenstander { get; set; } = string.Empty;
    public int AdminID { get; set; }

    // Navigation properties
    public Admin? Admin { get; set; }
    public ICollection<WedstrijdSpeler> WedstrijdSpelers { get; set; } = new List<WedstrijdSpeler>();
}
