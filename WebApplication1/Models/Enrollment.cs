using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Enrollment
    {
        [Required]
        public int Semester { get; set; }
        [Required]
        public String Studies { get; set; }
    }

}