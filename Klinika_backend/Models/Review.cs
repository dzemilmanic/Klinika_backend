namespace Klinika_backend.Models
{
    public class Review
    {
        public Guid Id { get; set; }
        public int Rating { get; set; }
        public string Content { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public DateTime UpdatedOn { get; set; }
        public string Author { get; set; }
    }
}
