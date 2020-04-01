using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EnrollmentsController : ControllerBase
    {
        private IStudentsDbService _dbService;

        public EnrollmentsController(IStudentsDbService service)
        {
            _dbService = service;
        }

        [HttpPost()]
        public IActionResult PostEnrollStudent(Student student)
        {
            if (_dbService.PostEnrollStudent(student) == null)
                return BadRequest("X");
            return Created("",_dbService.PostEnrollStudent(student));
        }

        [HttpPost("promotions")]
        public IActionResult PostPromoteStudent(Enrollment promote)
        {
            if (_dbService.PostPromoteStudents(promote) == null)
                return NotFound("X");
            return Created("", _dbService.PostPromoteStudents(promote));
        }
    }

}