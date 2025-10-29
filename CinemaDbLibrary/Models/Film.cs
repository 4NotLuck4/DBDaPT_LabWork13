using System.Text.Json.Serialization;

namespace CinemaDbLibrary.Models
{
    public class Film
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public short Duration { get; set; }
        public short ReleaseYear { get; set; }
        public string Description { get; set; }

        [JsonIgnore] // 5.1.3 Игнорирование постера
        public byte[] Poster { get; set; }

        public string AgeLimit { get; set; }
        public DateTime? RentalStart { get; set; }
        public DateTime? RentalFinish { get; set; }

        [JsonIgnore] // 5.1.3 Игнорирование навигационных свойств
        public ICollection<FilmGenre> FilmGenres { get; set; }

        [JsonIgnore]
        public ICollection<Session> Sessions { get; set; }
    }
}
