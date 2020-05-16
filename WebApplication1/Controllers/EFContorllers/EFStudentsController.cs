using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs.Requests;
using WebApplication1.EFModels;
using WebApplication1.EFServices;

namespace WebApplication1.Controllers.EFContorllers
{
    [ApiController]
    [Route("api/students")]
    public class EFStudentsController : ControllerBase
    {
        private IEFStudentsDbService _dbService;

        public EFStudentsController(IEFStudentsDbService service)
        {
            _dbService = service;
        }

        [HttpGet]
        public IActionResult GetStudents()
        {
            return Ok(_dbService.GetStudents());
        }

        [HttpPost]
        public IActionResult ModifyStudent([FromBody] Student student)
        {
            _dbService.ModifyStudent(student);
            return Ok("Updated student: " + student.IndexNumber);
        }

        [HttpDelete]
        public IActionResult DeleteStudent([FromBody] StudentDeleteRequest d)
        {
            _dbService.DeleteStudent(d);
            return Ok("Deleted student: " + d.IndexNumber + ": " + d.FirstName + " " + d.LastName);
        }

    }

}