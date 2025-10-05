using Microsoft.AspNetCore.Identity;

namespace Shiftly.Models;

public class ApplicationUser : IdentityUser
{
    public string Naam { get; set; } = "";
    public string Positie { get; set; } = ""; 
    public string Rugnummer { get; set; } = "";
    public string BeschikbaarheidJson { get; set; } = "";
    public int? TeamId { get; set; }
    public Team? Team { get; set; }
}
