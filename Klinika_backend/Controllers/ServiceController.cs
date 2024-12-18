using Klinika_backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Klinika_backend.Models.DTO;
using Klinika_backend.Models;

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
            var services = await _context.Services
                .Include(s => s.Category)
                .ToListAsync();

            // Mapiranje Service objekata u ServiceDto
            var serviceDtos = services.Select(s => new ServiceDto
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                CategoryId = s.CategoryId,
                CreatedOn = s.CreatedOn
            }).ToList();

            return Ok(serviceDtos); // Vraćaš listu DTO objekata
        }

        // GET: api/Service/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceDto>> GetService(Guid id)
        {
            var service = await _context.Services
                .Include(s => s.Category)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (service == null)
            {
                return NotFound();
            }

            // Mapiranje Service objekta u ServiceDto
            var serviceDto = new ServiceDto
            {
                Id = service.Id,
                Name = service.Name,
                Description = service.Description,
                CategoryId = service.CategoryId,
                CreatedOn = service.CreatedOn
            };

            return Ok(serviceDto);
        }

        // PUT: api/Service/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutService(Guid id, ServiceDto serviceDto)
        {
            if (id != serviceDto.Id)
            {
                return BadRequest();
            }

            var service = await _context.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound();
            }

            // Ažuriraj Service entitet sa podacima iz DTO
            service.Name = serviceDto.Name;
            service.Description = serviceDto.Description;
            service.CategoryId = serviceDto.CategoryId;

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
        public async Task<ActionResult<ServiceDto>> PostService(ServiceDto serviceDto)
        {
            var service = new Service
            {
                Id = Guid.NewGuid(),
                Name = serviceDto.Name,
                Description = serviceDto.Description,
                CategoryId = serviceDto.CategoryId,
                CreatedOn = DateTime.UtcNow
            };

            _context.Services.Add(service);
            await _context.SaveChangesAsync();

            // Mapiranje novog Service objekta u ServiceDto
            serviceDto.Id = service.Id;
            serviceDto.CreatedOn = service.CreatedOn;

            return CreatedAtAction("GetService", new { id = service.Id }, serviceDto);
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
