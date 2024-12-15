using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Klinika_backend.Models;
using Klinika_backend.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Klinika_backend.Data;

namespace Klinika_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController: ControllerBase
    {
        private readonly APP_DB_Context _context;

        public ReviewController(APP_DB_Context context)
        {
            _context = context;
        }

        // GET: api/Reviews
        [HttpGet]
        public async Task<IActionResult> GetReviews()
        {
            var reviews = await _context.Reviews.ToListAsync();
            return Ok(reviews);
        }

        // GET: api/Reviews/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReview(Guid id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound(new { Message = "Recenzija nije pronađena." });
            }

            return Ok(review);
        }

        // POST: api/Reviews
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview([FromBody] ReviewDto review)
        {
            // Dohvati ime trenutnog korisnika iz tokena
            var userName = User.FindFirstValue(ClaimTypes.Name);

            if (string.IsNullOrEmpty(userName))
            {
                return Unauthorized(new { Message = "Niste autorizovani za kreiranje recenzije." });
            }

            review.Id = Guid.NewGuid();
            review.Author = userName;
            review.CreatedOn = DateTime.UtcNow;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReview), new { id = review.Id }, review);
        }

        // PUT: api/Reviews/{id}
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(Guid id, [FromBody] ReviewDto updatedReview)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound(new { Message = "Recenzija nije pronađena." });
            }

            // Proveri da li trenutni korisnik može da ažurira recenziju
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (review.Author != userName)
            {
                return Forbid("Nemate dozvolu za brisanje ove recenzije.");

            }

            review.Content = updatedReview.Content;
            review.UpdatedOn = DateTime.UtcNow;

            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();

            return Ok(review);
        }


        // DELETE: api/Reviews/{id}
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(Guid id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound(new { Message = "Recenzija nije pronađena." });
            }

            // Proveri da li trenutni korisnik može da obriše recenziju
            var userName = User.FindFirstValue(ClaimTypes.Name);
            if (review.Author != userName)
            {
                return Forbid("Nemate dozvolu za brisanje ove recenzije.");

            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Recenzija je uspešno obrisana." });
        }

    }
}
