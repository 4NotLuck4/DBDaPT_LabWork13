using System.Text.Json.Serialization;

namespace CinemaDbLibrary.Models
{
    public class Session
    {
        public int Id { get; set; }
        public int FilmId { get; set; }
        public byte HallId { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsFilm3d { get; set; }

        [JsonIgnore]
        public Film Film { get; set; }

        [JsonIgnore]
        public CinemaHall CinemaHall { get; set; }

        [JsonIgnore]
        public ICollection<Ticket> Tickets { get; set; }
    }
}
