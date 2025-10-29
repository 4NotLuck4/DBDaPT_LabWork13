using System.Text.Json.Serialization;

namespace CinemaDbLibrary.Models
{
    public class CinemaHall
    {
        public byte Id { get; set; }
        public string CinemaName { get; set; }
        public byte HallNumber { get; set; }
        public byte RowsCount { get; set; }
        public byte SeatsPerRow { get; set; }
        public bool IsVip { get; set; }

        [JsonIgnore]
        public ICollection<Session> Sessions { get; set; }
    }
}
