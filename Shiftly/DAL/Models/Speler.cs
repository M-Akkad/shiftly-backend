namespace DAL.Models;

public class Speler
{
    public int ID { get; set; }
    public string Naam { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Positie { get; set; } = string.Empty;
    public int Rugnummer { get; set; }
    public string WachtwoordHash { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<WedstrijdSpeler> WedstrijdSpelers { get; set; } = new List<WedstrijdSpeler>();
}
