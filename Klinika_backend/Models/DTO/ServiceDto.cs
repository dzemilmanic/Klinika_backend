using System.ComponentModel.DataAnnotations;

namespace Klinika_backend.Models.DTO
{
    public class ServiceDto
    {
        public Guid Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        public decimal Price { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public Guid CategoryId { get; set; }
        public ServiceCategoryDto Category { get; set; }

    }
}
