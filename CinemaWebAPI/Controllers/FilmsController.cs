using CinemaDbLibrary.Context;
using CinemaDbLibrary.Models;
using CinemaWebAPI.Dto;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CinemaWebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilmsController : ControllerBase
    {
        private readonly CinemaDbContext _context;

        public FilmsController(CinemaDbContext context)
        {
            _context = context;
        }

        // GET: api/Films
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Film>>> GetFilms()
        {
            return await _context.Films.ToListAsync();
        }

        #region DLS





        // 5.2 GET-метод с параметрами запроса для постраничного вывода и сортировки
        [HttpGet("pages")]
        public async Task<ActionResult<IEnumerable<Film>>> GetFilmsPaged(
            [FromQuery] int page = 1,
            [FromQuery] string sortBy = "title")
        {
            const int pageSize = 3; // Размер страницы 3

            var query = _context.Films.AsQueryable();

            // Сортировка
            query = sortBy.ToLower() switch
            {
                "year_asc" => query.OrderBy(f => f.ReleaseYear),
                "year_desc" => query.OrderByDescending(f => f.ReleaseYear),
                _ => query.OrderBy(f => f.Title) // По умолчанию по названию
            };

            // Пагинация
            var films = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return Ok(films);
        }

        // 5.2 GET-метод с параметрами запроса для фильтрации
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<Film>>> GetFilmsFiltered(
            [FromQuery] int? year = null,
            [FromQuery] string title = null)
        {
            var query = _context.Films.AsQueryable();

            if (year.HasValue)
                query = query
                    .Where(f => f.ReleaseYear == year.Value);

            if (!string.IsNullOrEmpty(title))
                query = query
                    .Where(f => f.Title.Contains(title));

            var films = await query.ToListAsync();
            return Ok(films);
        }

        // 5.3 GET-метод с параметром пути для получения жанров фильма
        [HttpGet("{id}/genres")]
        public async Task<ActionResult<IEnumerable<Genre>>> GetFilmGenres(int id)
        {
            var genres = await _context.FilmGenres
                .Where(fg => fg.FilmId == id)
                .Include(fg => fg.Genre)
                .Select(fg => fg.Genre)
                .ToListAsync();

            if (!genres.Any())
                return NotFound($"Жанры для фильма с ID {id} не найдены");

            return Ok(genres);
        }

        // 5.3 GET-метод с параметром пути для получения будущих сеансов
        [HttpGet("{id}/sessions")]
        public async Task<ActionResult<IEnumerable<Session>>> GetFilmSessions(int id)
        {
            var currentDate = DateTime.Now;

            var sessions = await _context.Sessions
                .Where(s => s.FilmId == id && s.StartDate > currentDate)
                .OrderBy(s => s.StartDate)
                .ToListAsync();

            if (!sessions.Any())
                return NotFound($"Будущие сеансы для фильма с ID {id} не найдены");

            return Ok(sessions);
        }

        // 5.4 GET-метод с составными параметрами запроса
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Film>>> SearchFilms(
            [FromQuery] string year = null,
            [FromQuery] string genres = null)
        {
            var query = _context.Films.AsQueryable();

            // Обработка диапазона годов
            if (!string.IsNullOrEmpty(year) && year.Contains('-'))
            {
                var years = year.Split('-');
                if (years.Length == 2 && int.TryParse(years[0], out int minYear) &&
                    int.TryParse(years[1], out int maxYear))
                {
                    query = query.Where(f => f.ReleaseYear >= minYear && f.ReleaseYear <= maxYear);
                }
            }

            // Обработка списка жанров
            if (!string.IsNullOrEmpty(genres))
            {
                var genreList = genres.Split(',').Select(g => g.Trim()).ToList();

                query = query.Where(f => f.FilmGenres
                    .Any(fg => genreList.Contains(fg.Genre.Name)));
            }

            var films = await query
                .Include(f => f.FilmGenres)
                    .ThenInclude(fg => fg.Genre)
                .ToListAsync();

            return Ok(films);
        }



        // CinemaWebAPI/Controllers/FilmsController.cs (продолжение)
        // 5.5 GET-методы, возвращающие DTO со статистикой
        [HttpGet("statistics")]
        public async Task<ActionResult<IEnumerable<FilmDto>>> GetFilmsStatistics()
        {
            var filmsStats = await _context.Films
                .Select(f => new FilmDto
                {
                    Id = f.Id,
                    Title = f.Title,
                    TicketsCount = f.Sessions
                        .SelectMany(s => s.Tickets)
                        .Count(),
                    SalesProfit = f.Sessions
                        .SelectMany(s => s.Tickets)
                        .Sum(t => t.Session.Price)
                })
                .Where(f => f.TicketsCount > 0) // Только фильмы с проданными билетами
                .ToListAsync();

            return Ok(filmsStats);
        }

        // 5.5 GET-метод с параметром пути для статистики конкретного фильма
        [HttpGet("statistics/{id}")]
        public async Task<ActionResult<FilmDto>> GetFilmStatistics(int id)
        {
            var filmStats = await _context.Films
                .Where(f => f.Id == id)
                .Select(f => new FilmDto
                {
                    Id = f.Id,
                    Title = f.Title,
                    TicketsCount = f.Sessions
                        .SelectMany(s => s.Tickets)
                        .Count(),
                    SalesProfit = f.Sessions
                        .SelectMany(s => s.Tickets)
                        .Sum(t => t.Session.Price)
                })
                .FirstOrDefaultAsync();

            if (filmStats == null)
                return NotFound($"Фильм с ID {id} не найден");

            return Ok(filmStats);
        }


        #endregion

        // GET: api/Films/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Film>> GetFilm(int id)
        {
            var film = await _context.Films.FindAsync(id);

            if (film == null)
            {
                return NotFound();
            }

            return film;
        }

        // PUT: api/Films/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutFilm(int id, Film film)
        {
            if (id != film.Id)
            {
                return BadRequest();
            }

            _context.Entry(film).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FilmExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Films
        [HttpPost]
        public async Task<ActionResult<Film>> PostFilm(Film film)
        {
            _context.Films.Add(film);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetFilm", new { id = film.Id }, film);
        }

        // DELETE: api/Films/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFilm(int id)
        {
            var film = await _context.Films.FindAsync(id);
            if (film == null)
            {
                return NotFound();
            }

            _context.Films.Remove(film);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FilmExists(int id)
        {
            return _context.Films.Any(e => e.Id == id);
        }
    }
}
