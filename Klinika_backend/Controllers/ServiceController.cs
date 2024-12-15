using Klinika_backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Klinika_backend.Models.DTO;

namespace Klinika_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController: ControllerBase
    {
        private readonly APP_DB_Context _context;

        public ServiceController(APP_DB_Context context)
        {
            _context = context;
        }
        // GET: api/Service
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceDto>>> GetServices()
        {
            return await _context.Services.Include(s => s.Category).ToListAsync();
        }
        // GET: api/Service/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceDto>> GetService(Guid id)
        {
            var service = await _context.Services.Include(s => s.Category)
                                                 .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
            {
                return NotFound();
            }

            return service;
        }
        // PUT: api/Service/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutService(Guid id, ServiceDto service)
        {
            if (id != service.Id)
            {
                return BadRequest();
            }

            _context.Entry(service).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceExists(id))
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

        // POST: api/Service
        [HttpPost]
        public async Task<ActionResult<ServiceDto>> PostService(ServiceDto service)
        {
            service.Id = Guid.NewGuid(); 
            service.CreatedOn = DateTime.UtcNow;

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetService", new { id = service.Id }, service);
        }

        // DELETE: api/Service/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteService(Guid id)
        {
            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            _context.Services.Remove(service);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceExists(Guid id)
        {
            return _context.Services.Any(e => e.Id == id);
        }
    }
}
