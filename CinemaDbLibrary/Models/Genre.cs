using System.Text.Json.Serialization;

namespace CinemaDbLibrary.Models
{
    public class Genre
    {
        public int Id { get; set; }
        public string Name { get; set; }

        [JsonIgnore] // 5.1.3 Игнорирование при сериализации
        public ICollection<FilmGenre> FilmGenres { get; set; }
    }
}
