using Klinika_backend.Models.DTO;
using System.ComponentModel.DataAnnotations;

namespace Klinika_backend.Models
{
    public class Service
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
