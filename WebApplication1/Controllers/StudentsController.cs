using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("api/students")]
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