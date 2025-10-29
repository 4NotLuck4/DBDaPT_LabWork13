using System.Text.Json.Serialization;

namespace CinemaDbLibrary.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        public int SessionId { get; set; }
        public int VisitorId { get; set; }
        public byte Row { get; set; }
        public byte Seat { get; set; }

        [JsonIgnore]
        public Session Session { get; set; }

        [JsonIgnore]
        public Visitor Visitor { get; set; }
    }
}
