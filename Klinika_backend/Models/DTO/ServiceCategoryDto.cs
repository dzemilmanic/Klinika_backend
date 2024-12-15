using System.ComponentModel.DataAnnotations;

namespace Klinika_backend.Models.DTO
{
    public class ServiceCategoryDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
    }
}
