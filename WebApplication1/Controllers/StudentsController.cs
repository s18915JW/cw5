using Microsoft.AspNetCore.Mvc;
using WebApplication1.Services;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Authorize(Roles = "Employee")]
    [Route("api-old/students")]
    public class StudentsController : ControllerBase
    {
        private IStudentsDbService _dbService;

        public StudentsController(IStudentsDbService service)
        {
            _dbService = service;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            return Ok(_dbService.GetStudents());
        }

        [HttpGet("{id}")]
        public IActionResult GetStudent(string id)
        {
            if (_dbService.GetStudent(id) != null)
                return Ok(_dbService.GetStudent(id));
            return NotFound("Nie znaleziono studenta");
        }
    }

}