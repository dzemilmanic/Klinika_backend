using System.ComponentModel.DataAnnotations;

namespace Klinika_backend.Models.DTO
{
    public class CheckEmailRequest
    {
        [Required(ErrorMessage = "Email je obavezan")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

    }
}
