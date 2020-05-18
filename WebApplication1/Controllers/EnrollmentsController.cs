using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs.Requests;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Authorize(Roles = "Employee")]
    [Route("api-old/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentsDbService _dbService;

        public EnrollmentsController(IStudentsDbService service)
        {
            _dbService = service;
        }

        [HttpPost()]
        public IActionResult PostEnrollStudent(StudentEnrollmentRequest s)
        {
            Enrollment e = _dbService.PostEnrollStudent(s);
            if (e == null)
                return BadRequest("X");
            return Created("", e);
        }

        [HttpPost("promotions")]
        public IActionResult PostPromoteStudent(PromotionRequest p)
        {
            Enrollment e = _dbService.PostPromoteStudents(p);
            if (e == null)
                return BadRequest("X");
            return Created("", e);
        }
    }

}