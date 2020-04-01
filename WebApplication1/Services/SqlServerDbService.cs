using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public class SqlServerDbService : IStudentsDbService
    {
        private const string ConString = "Data Source=db-mssql;Initial Catalog=s18915;Integrated Security=True";

        public IEnumerable<Student> GetStudents()
        {
            var list = new List<Student>();

            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText =
                    "select IndexNumber, FirstName, LastName, BirthDate, Name, Semester from student " +
                    "inner join Enrollment on Enrollment.IdEnrollment = Student.IdEnrollment " +
                    "inner join Studies on Studies.IdStudy = Enrollment.IdStudy;";

                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    list.Add(new Student
                    {
                        IndexNumber = dr["IndexNumber"].ToString(),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = Convert.ToDateTime(dr["BirthDate"]),
                        StudiesName = dr["Name"].ToString(),
                        Semester = Convert.ToInt32(dr["Semester"])
                    });
                }
            }
            return list;
        }

        public Student GetStudent(string id)
        {
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                com.CommandText =
                    "select IndexNumber, FirstName, LastName, BirthDate, Semester, Name from enrollment " +
                    "join Studies on Studies.IdStudy = Enrollment.IdEnrollment " +
                    "join Student on Student.IdEnrollment = Enrollment.IdEnrollment " +
                    "where IndexNumber = @index";

                com.Parameters.AddWithValue("index", id);

                con.Open();
                var dr = com.ExecuteReader();
                while (dr.Read())
                {
                    var st = new Student
                    {
                        IndexNumber = dr["IndexNumber"].ToString(),
                        FirstName = dr["FirstName"].ToString(),
                        LastName = dr["LastName"].ToString(),
                        BirthDate = Convert.ToDateTime(dr["BirthDate"]),
                        Semester = Convert.ToInt32(dr["Semester"]),
                        StudiesName = dr["Name"].ToString()
                    };
                    return st;
                }
            }
            return null;
        }



        public Enrollment PostEnrollStudent([FromBody] Student student)
        {
            Enrollment res = null;  // later if still null in controller response will be 400

            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();

                var tran = con.BeginTransaction();
                com.Transaction = tran;

                try // try for all possible errors
                {               
                    // studies 
                    com.Parameters.AddWithValue("StudiesName", student.StudiesName);
                    com.CommandText = 
                        "SELECT IdStudy FROM Studies WHERE Name=@StudiesName";
                    var dr = com.ExecuteReader();
                    dr.Read();
                    int IdStudy = Convert.ToInt32(dr["IdStudy"]);  
                    dr.Close();
                    com.Parameters.AddWithValue("IdStudy", IdStudy);

                    // enrollment (newest enroll for semester = 1 and IdStudy = @IdStudy)
                    com.CommandText =
                        "SELECT IdEnrollment, StartDate FROM Enrollment" +
                        " WHERE IdStudy = @IdStudy AND Semester = 1" +
                        " AND StartDate = (SELECT MAX(StartDate) FROM Enrollment WHERE IdStudy = @IdStudy AND Semester = 1);";
                    dr = com.ExecuteReader();
                    dr.Read();
                    var IdEnrollment = dr["IdEnrollment"]; // maybe not exists
                    dr.Close();

                    int EId = 0;    // IdEnrollment for insert
                    if (IdEnrollment == null)
                    {
                        // new Enrollment
                        string StartDate = (DateTime.Now).ToString("YYYY-mm-DD");
                        com.Parameters.AddWithValue("@StartDate", StartDate);
                        com.CommandText = "INSERT INTO Enrollment VALUES((SELECT MAX(IdEnrollment) + 1 FROM Enrollment), 1, @IdStudy, @StartDate);";
                        com.ExecuteNonQuery();

                        com.CommandText = "SELECT MAX(IdEnrollment) FROM Enrollment";
                        dr = com.ExecuteReader();
                        dr.Read();
                        EId = Convert.ToInt32(dr["IdEnrollment"]);
                        dr.Close();
                    }
                    else 
                    {
                        EId = Convert.ToInt32(IdEnrollment);
                    }
                    com.Parameters.AddWithValue("IdEnrollment", EId);

                    // student
                    com.Parameters.AddWithValue("Index", student.IndexNumber); 
                    com.Parameters.AddWithValue("FName", student.FirstName);
                    com.Parameters.AddWithValue("LName", student.LastName);
                    com.Parameters.AddWithValue("BDate", DateTime.Parse(student.BirthDate.ToString())); 

                    com.CommandText = "INSERT INTO Student VALUES(@Index, @Fname, @LName, @BDate, @IdEnrollment);";
                    com.ExecuteNonQuery();

                    tran.Commit();

                    // no exception thrown so ok 201
                    res = new Enrollment {
                        Semester = 1,
                        Studies = student.StudiesName
                    };
                }
                catch (SqlException)
                {
                    tran.Rollback();
                }
                return res;
            }
        }

        public Enrollment PostPromoteStudents(Enrollment promote)
        {
            Enrollment res = null;

            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand("PromoteStudents", con))
            {
                com.Connection = con;
                con.Open();

                var tran = con.BeginTransaction();
                com.Transaction = tran;

                try
                {
                    com.Parameters.AddWithValue("Studies", promote.Studies);
                    com.Parameters.AddWithValue("Semester", promote.Semester);

                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.ExecuteNonQuery();

                    tran.Commit();
                    res = new Enrollment
                    {
                        Semester = promote.Semester + 1,
                        Studies = promote.Studies
                    };
                }
                catch (SqlException)
                {
                    tran.Rollback();
                }
                return res;
            }
        }
    }

}