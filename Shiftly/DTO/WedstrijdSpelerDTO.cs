namespace DTO
{
    public class WedstrijdSpelerDTO
    {
        public int WedstrijdID { get; set; }
        public int SpelerID { get; set; }
        public string Status { get; set; }  // "Aanwezig" of "Afwezig"

        // Navigation properties
        public WedstrijdDTO? Wedstrijd { get; set; }
        public SpelerDTO? Speler { get; set; }
    }
}