namespace ExchangeDesk.Web.Models
{
    public class Office
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool IsCentral { get; set; }
        // указва дали е централен офис
        public string Location { get; set; } = null!; // локация на офиса
    }
}
