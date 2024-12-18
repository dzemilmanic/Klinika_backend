using Klinika_backend.Data;
using Klinika_backend.Models;
using Klinika_backend.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Klinika_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "User")]
    public class AppointmentController : ControllerBase
    {
        private readonly APP_DB_Context _context;

        public AppointmentController(APP_DB_Context context)
        {
            _context = context;
        }

        // GET: api/Appointment
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AppointmentDto>>> GetAppointments()
        {
            var appointments = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Patient)
                .ToListAsync();

            // Mapiranje Appointment objekata u AppointmentDto
            var appointmentDtos = appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                ServiceId = a.ServiceId,
                PatientId = a.PatientId,
                AppointmentDate = a.AppointmentDate,
                CreatedOn = a.CreatedOn
            }).ToList();

            return Ok(appointmentDtos); // Vraćaš listu DTO objekata
        }

        // GET: api/Appointment/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AppointmentDto>> GetAppointment(Guid id)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Service)
                .Include(a => a.Patient)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            // Mapiranje Appointment objekta u AppointmentDto
            var appointmentDto = new AppointmentDto
            {
                Id = appointment.Id,
                ServiceId = appointment.ServiceId,
                PatientId = appointment.PatientId,
                AppointmentDate = appointment.AppointmentDate,
                CreatedOn = appointment.CreatedOn
            };

            return Ok(appointmentDto);
        }

        // PUT: api/Appointment/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(Guid id, AppointmentDto appointmentDto)
        {
            if (id != appointmentDto.Id)
            {
                return BadRequest();
            }

            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            // Ažuriraj Appointment entitet sa podacima iz DTO
            appointment.ServiceId = appointmentDto.ServiceId;
            appointment.PatientId = appointmentDto.PatientId;
            appointment.AppointmentDate = appointmentDto.AppointmentDate;

            _context.Entry(appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(id))
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

        // POST: api/Appointment
        [HttpPost]
        public async Task<ActionResult<AppointmentDto>> PostAppointment(AppointmentDto appointmentDto)
        {
            var appointment = new Appointment
            {
                Id = Guid.NewGuid(),
                ServiceId = appointmentDto.ServiceId,
                PatientId = appointmentDto.PatientId,
                AppointmentDate = appointmentDto.AppointmentDate,
                CreatedOn = DateTime.UtcNow
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            // Mapiranje novog Appointment objekta u AppointmentDto
            appointmentDto.Id = appointment.Id;
            appointmentDto.CreatedOn = appointment.CreatedOn;

            return CreatedAtAction("GetAppointment", new { id = appointment.Id }, appointmentDto);
        }

        // DELETE: api/Appointment/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(Guid id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AppointmentExists(Guid id)
        {
            return _context.Appointments.Any(e => e.Id == id);
        }
    }
}