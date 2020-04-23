using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IStudentsDbService
    {
        public IEnumerable<Student> GetStudents();
        public Student GetStudent(string id);
        public Enrollment PostEnrollStudent([FromBody] Student student);
        public Enrollment PostPromoteStudents([FromBody] Enrollment promote);
        public bool Login([FromBody] LoginRequestDto loginRequest);
        public void UpdateRefreshToken(string token, string login);
        public bool GetRefreshToken(string token);
    }

}