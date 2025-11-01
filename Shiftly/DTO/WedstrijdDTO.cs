namespace DTO
{
    public class WedstrijdDTO
    {
        public int ID { get; set; }
        public DateTime Datum { get; set; }
        public TimeSpan Tijd { get; set; }
        public string Locatie { get; set; }
        public string Tegenstander { get; set; }
        public int AdminID { get; set; }

        // Navigation properties
        public AdminDTO? Admin { get; set; }
        public List<WedstrijdSpelerDTO>? WedstrijdSpelers { get; set; }
    }
}