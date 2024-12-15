using System.ComponentModel.DataAnnotations;

namespace Klinika_backend.Models.DTO
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }

        [Required]
        public Guid ServiceId { get; set; }  
        public ServiceDto Service { get; set; }

        [Required]
        public Guid PatientId { get; set; }
        public ApplicationUser Patient { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
