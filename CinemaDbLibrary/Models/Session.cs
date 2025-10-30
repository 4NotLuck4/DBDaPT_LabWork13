using System;
using System.Collections.Generic;

namespace CinemaDbLibrary.Models;

public partial class Session
{
    public int SessionId { get; set; }

    public int? FilmId { get; set; }

    public byte? HallId { get; set; }

    public decimal? Price { get; set; }

    public DateTime? StartDate { get; set; }

    public bool? Is3d { get; set; }

    public virtual Film? Film { get; set; }

    public virtual Hall? Hall { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
