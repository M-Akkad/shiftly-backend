namespace DAL.Models;

public class WedstrijdSpeler
{
    public int WedstrijdID { get; set; }
    public int SpelerID { get; set; }
    public string Status { get; set; } = "Aanwezig"; // "Aanwezig" of "Afwezig"

    // Navigation properties
    public Wedstrijd? Wedstrijd { get; set; }
    public Speler? Speler { get; set; }
}
