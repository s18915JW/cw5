using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs.Requests
{
    public class StudentEnrollmentRequest
    {
        [Required]
        public string IndexNumber { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateTime BirthDate { get; set; }
        [Required]
        public string StudiesName { get; set; }
    }
}
