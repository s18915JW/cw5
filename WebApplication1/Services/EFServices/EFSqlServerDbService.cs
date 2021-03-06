﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using WebApplication1.DTOs.Requests;
using WebApplication1.EFModels;

namespace WebApplication1.EFServices
{
    public class EFSqlServerDbService : IEFStudentsDbService
    {
        public IEnumerable<Student> GetStudents()
        {
            var db = new s18915Context();
            return db.Student.ToList();
        }

        public void ModifyStudent([FromBody] Student student)
        {
            var db = new s18915Context();
            var s = new Student
            {
                IndexNumber = student.IndexNumber,
                FirstName = student.FirstName,
                LastName = student.LastName,
            };
            db.Attach(s);
            db.Entry(s).Property("FirstName").IsModified = true;
            db.Entry(s).Property("LastName").IsModified = true;
            db.SaveChanges();
        }

        public void DeleteStudent([FromBody] StudentDeleteRequest del)
        {
            var db = new s18915Context();
            Student student = db.Student.SingleOrDefault(x => (x.IndexNumber == del.IndexNumber && x.FirstName == del.FirstName && x.LastName == del.LastName));
            db.Student.Remove(student);
            db.SaveChanges();
        }

        public Enrollment PostEnrollStudent([FromBody] StudentEnrollmentRequest s)
        {
            var db = new s18915Context();

            // StudentEnrollRequest has fields [Required] - so check for fields done

            // check if studies exists
            if (!db.Studies.Any(x => x.Name == s.StudiesName))
                return null;   

            // check if student's index is unique
            if (db.Student.Any(x => x.IndexNumber == s.IndexNumber))
                return null;

            var studies = db.Studies.FirstOrDefault(x => x.Name == s.StudiesName);
            var enrollment = db.Enrollment.FirstOrDefault(x => x.Semester == 1 && x.IdStudy == studies.IdStudy);

            var id = 0;

            // enrollment does not exist
            if (enrollment == null)
            {
                // get the latest one
                var maxId = db.Enrollment
                    .OrderByDescending(x => x.IdEnrollment)
                    .FirstOrDefault().IdEnrollment;

                id = maxId + 1;

                db.Enrollment.Add(new Enrollment
                {
                    IdEnrollment = id,
                    Semester = 1,
                    IdStudy = studies.IdStudy,
                    StartDate = DateTime.Now
                });
            }
            if (enrollment != null)
                id = enrollment.IdEnrollment;
            
            db.Student.Add(new Student
            {
                IndexNumber = s.IndexNumber,
                FirstName = s.FirstName,
                LastName = s.LastName,
                BirthDate = s.BirthDate,
                IdEnrollment = id
            });
            db.SaveChanges();

            return new Enrollment
            {
                IdEnrollment = id,
                Semester = 1,
                IdStudy = studies.IdStudy,
                StartDate = enrollment == null ? DateTime.Now : enrollment.StartDate
            };
        }

        public Enrollment PostPromoteStudents([FromBody] PromotionRequest p)
        {
            var db = new s18915Context();

            //db.Database.ExecuteSqlRaw("EXEC PromoteStudents", new SqlParameter("name", p.Studies), new SqlParameter("sem", p.Semester));
            //return new Enrollment
            //{
            //    Semester = p.Semester + 1,
            //    IdStudy = db.Studies.OrderByDescending(x => x.IdStudy).FirstOrDefault(x => x.Name == p.Studies).IdStudy
            //};


            // check for studies
            if (!db.Studies.Any(x => x.Name == p.Studies))
                return null;

            var studies = db.Studies.FirstOrDefault(x => x.Name == p.Studies);

            // check if enrollment like that exists
            if (!db.Enrollment.Any(x => x.Semester == p.Semester && x.IdStudy == studies.IdStudy))
                return null;            
            
            // search for record in enrollment with semester + 1
            var nextEnrollment = db.Enrollment.FirstOrDefault(x => x.IdStudy == studies.IdStudy && x.Semester == p.Semester + 1);
            int newId = 0;
            
            // insert new enrollment if not found
            if (nextEnrollment == null)
            {
                var maxId = db.Enrollment
                   .OrderByDescending(x => x.IdStudy)
                   .FirstOrDefault().IdEnrollment;

                newId = maxId + 1;

                db.Add(new Enrollment
                {
                    IdEnrollment = maxId + 1,
                    Semester = p.Semester + 1,
                    IdStudy = studies.IdStudy,
                    StartDate = DateTime.Now
                });
            }
            if (nextEnrollment != null)
                newId = nextEnrollment.IdEnrollment;

            var toUpdate = db.Student.Where(x => x.IdEnrollment == db.Enrollment.FirstOrDefault(x => x.Semester == p.Semester && x.IdStudy == studies.IdStudy).IdEnrollment);
            foreach (var student in toUpdate)
                student.IdEnrollment = newId;

            db.SaveChanges();

            return new Enrollment
            {
                IdEnrollment = newId,
                Semester = p.Semester + 1,
                IdStudy = studies.IdStudy,
                StartDate = nextEnrollment == null ? DateTime.Now : nextEnrollment.StartDate
            };
        }

    }

}