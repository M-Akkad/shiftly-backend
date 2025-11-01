namespace DAL.Models;

public class Admin
{
    public int ID { get; set; }
    public string Naam { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string WachtwoordHash { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Wedstrijd> Wedstrijden { get; set; } = new List<Wedstrijd>();
}
