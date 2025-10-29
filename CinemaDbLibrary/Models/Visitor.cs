using System.Text.Json.Serialization;

namespace CinemaDbLibrary.Models
{
    public class Visitor
    {
        public int Id { get; set; }
        public string Phone { get; set; }
        public string Name { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        public ICollection<Ticket> Tickets { get; set; }
    }
}
