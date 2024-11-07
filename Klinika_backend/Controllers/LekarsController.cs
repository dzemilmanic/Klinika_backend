using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Klinika_backend.Data;
using Klinika_backend.Models;

namespace Klinika_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LekarsController : ControllerBase
    {
        private readonly APP_DB_Context _context;

        public LekarsController(APP_DB_Context context)
        {
            _context = context;
        }

        // GET: api/Lekars
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Lekar>>> GetLekar()
        {
            return await _context.Lekar.ToListAsync();
        }

        // GET: api/Lekars/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Lekar>> GetLekar(Guid id)
        {
            var lekar = await _context.Lekar.FindAsync(id);

            if (lekar == null)
            {
                return NotFound();
            }

            return lekar;
        }

        // PUT: api/Lekars/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLekar(Guid id, Lekar lekar)
        {
            if (id != lekar.Id)
            {
                return BadRequest();
            }

            _context.Entry(lekar).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LekarExists(id))
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

        // POST: api/Lekars
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Lekar>> PostLekar(Lekar lekar)
        {
            _context.Lekar.Add(lekar);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLekar", new { id = lekar.Id }, lekar);
        }

        // DELETE: api/Lekars/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLekar(Guid id)
        {
            var lekar = await _context.Lekar.FindAsync(id);
            if (lekar == null)
            {
                return NotFound();
            }

            _context.Lekar.Remove(lekar);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LekarExists(Guid id)
        {
            return _context.Lekar.Any(e => e.Id == id);
        }
    }
}
