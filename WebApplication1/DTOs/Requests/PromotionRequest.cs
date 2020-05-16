using System;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTOs.Requests
{
    public class PromotionRequest
    {
        [Required]
        public int Semester { get; set; }
        [Required]
        public String Studies { get; set; }
    }
}
