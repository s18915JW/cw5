using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using WebApplication1.DTOs.Requests;
using WebApplication1.EFModels;

namespace WebApplication1.EFServices
{
    public interface IEFStudentsDbService
    {
        public IEnumerable<Student> GetStudents();
        public void ModifyStudent([FromBody] Student s);
        public void DeleteStudent([FromBody] StudentDeleteRequest d);

        public Enrollment PostEnrollStudent([FromBody] StudentEnrollmentRequest s);
        public Enrollment PostPromoteStudents([FromBody] PromotionRequest p);
    }

}