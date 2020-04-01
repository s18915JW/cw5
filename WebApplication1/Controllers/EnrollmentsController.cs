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

        [HttpPost]
        public IActionResult PostEnrollStudent(Enrollment enrollment)
        {
            return Created("", null);
        }

        [HttpPost]
        public IActionResult PostPromoteStudent(int Semester, string Studies)
        {
            return Created("", null);
        }

    }
}