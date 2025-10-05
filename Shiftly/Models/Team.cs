namespace Shiftly.Models;

public class Team
{
    public int Id { get; set; }
    public string Naam { get; set; } = "";
    public ICollection<ApplicationUser> Leden { get; set; } = new List<ApplicationUser>();
}
