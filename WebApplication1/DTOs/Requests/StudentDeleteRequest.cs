using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs.Requests
{
    public class StudentDeleteRequest
    {
        [Required]
        public string IndexNumber { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
    }
}
