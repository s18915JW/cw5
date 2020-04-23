using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Student
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
        public int Semester { get; set; }
    }

}