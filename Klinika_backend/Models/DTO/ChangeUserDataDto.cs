namespace Klinika_backend.Models.DTO
{
    public class ChangeUserDataDto
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }
    }
    
}
