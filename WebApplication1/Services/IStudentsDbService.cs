using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApplication1.DTO;
using WebApplication1.DTOs.Requests;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public interface IStudentsDbService
    {
        public IEnumerable<Student> GetStudents();
        public Student GetStudent(string id);
        public Enrollment PostEnrollStudent([FromBody] StudentEnrollmentRequest s);
        public Enrollment PostPromoteStudents([FromBody] PromotionRequest p);
        public bool Login([FromBody] LoginRequest loginRequest);
        public void UpdateRefreshToken(string token, string login);
        public bool GetRefreshToken(string token);
    }

}