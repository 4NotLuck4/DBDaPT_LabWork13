using System.Text.Json.Serialization;

namespace CinemaDbLibrary.Models
{
    public class FilmGenre
    {
        public int FilmId { get; set; }
        public int GenreId { get; set; }

        [JsonIgnore]
        public Film Film { get; set; }

        [JsonIgnore]
        public Genre Genre { get; set; }
    }
}
