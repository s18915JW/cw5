using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IStudentsDbService
    {
        // cw4 copy
        public IEnumerable<Student> GetStudents();
        public Student GetStudent(string id);
        // cw5 new
        public Enrollment PostEnrollStudent([FromBody] Student student);
        public Enrollment PostPromoteStudents([FromBody] Enrollment promote);
    }
}