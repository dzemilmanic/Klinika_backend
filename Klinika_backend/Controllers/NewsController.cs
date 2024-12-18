using Klinika_backend.Models.DTO;
using Klinika_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Klinika_backend.Data;
using Microsoft.EntityFrameworkCore;

namespace Klinika_backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NewsController : ControllerBase
    {
        private readonly APP_DB_Context _context;

        public NewsController(APP_DB_Context context)
        {
            _context = context;
        }

        // GET: api/News
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NewsDto>>> GetAllNews()
        {
            var news = await _context.News
                .Select(n => new NewsDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    PublishedDate = n.PublishedDate
                })
                .ToListAsync();

            return Ok(news);
        }

        // GET: api/News/5
        [HttpGet("{id}")]
        public async Task<ActionResult<NewsDto>> GetNewsById(int id)
        {
            var news = await _context.News
                .Where(n => n.Id == id)
                .Select(n => new NewsDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    Content = n.Content,
                    PublishedDate = n.PublishedDate
                })
                .FirstOrDefaultAsync();

            if (news == null)
            {
                return NotFound();
            }

            return Ok(news);
        }

        // POST: api/News
        [HttpPost]
        public async Task<ActionResult<NewsDto>> AddNews([FromBody] NewsDto newsDto)
        {
            var news = new News
            {
                Title = newsDto.Title,
                Content = newsDto.Content,
                PublishedDate = newsDto.PublishedDate
            };

            _context.News.Add(news);
            await _context.SaveChangesAsync();

            // Mapiranje News entiteta na NewsDto
            var newsDtoResponse = new NewsDto
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                PublishedDate = news.PublishedDate
            };

            return CreatedAtAction(nameof(GetNewsById), new { id = news.Id }, newsDtoResponse);
        }

        // PUT: api/News/5
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateNews(int id, [FromBody] NewsDto newsDto)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            news.Title = newsDto.Title;
            news.Content = newsDto.Content;
            news.PublishedDate = newsDto.PublishedDate;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/News/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteNews(int id)
        {
            var news = await _context.News.FindAsync(id);
            if (news == null)
            {
                return NotFound();
            }

            _context.News.Remove(news);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
