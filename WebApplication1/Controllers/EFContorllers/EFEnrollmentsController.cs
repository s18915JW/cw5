using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs.Requests;
using WebApplication1.EFModels;
using WebApplication1.EFServices;

namespace WebApplication1.Controllers.EFContorllers
{
    [ApiController]
    [Route("api/enrollments")]
    public class EFEnrollmentsController : ControllerBase
    {
        private IEFStudentsDbService _dbService;

        public EFEnrollmentsController(IEFStudentsDbService service)
        {
            _dbService = service;
        }

        [HttpPost()]
        public IActionResult PostEnrollStudent([FromBody] StudentEnrollmentRequest s)
        {
            Enrollment e = _dbService.PostEnrollStudent(s);
            if (e == null)
                return BadRequest("X");
            return Created("", e);
        }

        [HttpPost("promotions")]
        public IActionResult PostPromoteStudent([FromBody] PromotionRequest p)
        {
            Enrollment e = _dbService.PostPromoteStudents(p);
            if (e == null)
                return BadRequest("X");
            return Created("", e);
        }
    }

}