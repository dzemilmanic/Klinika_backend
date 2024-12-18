using Klinika_backend.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Klinika_backend.Models.DTO;
using Microsoft.AspNetCore.Authorization;

namespace Klinika_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ServiceCategoryController : ControllerBase
    {
        private readonly APP_DB_Context _context;

        public ServiceCategoryController(APP_DB_Context context)
        {
            _context = context;
        }

        // GET: api/ServiceCategory
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceCategoryDto>>> GetServiceCategories()
        {
            return await _context.ServiceCategories.ToListAsync();
        }

        // GET: api/ServiceCategory/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceCategoryDto>> GetServiceCategory(Guid id)
        {
            var serviceCategory = await _context.ServiceCategories.FindAsync(id);

            if (serviceCategory == null)
            {
                return NotFound();
            }

            return serviceCategory;
        }

        // PUT: api/ServiceCategory/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceCategory(Guid id, ServiceCategoryDto serviceCategory)
        {
            if (id != serviceCategory.Id)
            {
                return BadRequest();
            }

            _context.Entry(serviceCategory).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceCategoryExists(id))
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

        // POST: api/ServiceCategory
        [HttpPost]
        public async Task<ActionResult<ServiceCategoryDto>> PostServiceCategory(ServiceCategoryDto serviceCategory)
        {
            serviceCategory.Id = Guid.NewGuid();  // Generišemo novi ID za kategoriju
            serviceCategory.CreatedOn = DateTime.UtcNow;

            _context.ServiceCategories.Add(serviceCategory);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetServiceCategory", new { id = serviceCategory.Id }, serviceCategory);
        }

        // DELETE: api/ServiceCategory/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceCategory(Guid id)
        {
            var serviceCategory = await _context.ServiceCategories.FindAsync(id);
            if (serviceCategory == null)
            {
                return NotFound();
            }

            _context.ServiceCategories.Remove(serviceCategory);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ServiceCategoryExists(Guid id)
        {
            return _context.ServiceCategories.Any(e => e.Id == id);
        }
    }
}
