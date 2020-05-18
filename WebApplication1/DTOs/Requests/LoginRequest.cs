using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class LoginRequest
    {
        [Required]
        public string Login { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
