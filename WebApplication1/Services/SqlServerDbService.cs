﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using WebApplication1.DTO;
using WebApplication1.DTOs.Requests;
using WebApplication1.Encryptors;
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
                    "join Studies on Studies.IdStudy = Enrollment.IdStudy " +
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

        public Enrollment PostEnrollStudent([FromBody] StudentEnrollmentRequest s)
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
                    com.Parameters.AddWithValue("StudiesName", s.StudiesName);
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
                    com.Parameters.AddWithValue("Index", s.IndexNumber);
                    com.Parameters.AddWithValue("FName", s.FirstName);
                    com.Parameters.AddWithValue("LName", s.LastName);
                    com.Parameters.AddWithValue("BDate", DateTime.Parse(s.BirthDate.ToString()));

                    com.CommandText = "INSERT INTO Student VALUES(@Index, @Fname, @LName, @BDate, @IdEnrollment);";
                    com.ExecuteNonQuery();

                    tran.Commit();
                    tran.Dispose();
                    // no exception thrown so ok 201
                    res = new Enrollment
                    {
                        Semester = 1,
                        Studies = s.StudiesName
                    };
                }
                catch (SqlException)
                {
                    tran.Rollback();
                }
                return res;
            }
        }

        public Enrollment PostPromoteStudents([FromBody] PromotionRequest p)
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
                    com.Parameters.AddWithValue("Studies", p.Studies);
                    com.Parameters.AddWithValue("Semester", p.Semester);

                    com.CommandType = System.Data.CommandType.StoredProcedure;
                    com.ExecuteNonQuery();

                    tran.Commit();
                    tran.Dispose();
                    res = new Enrollment
                    {
                        Semester = p.Semester + 1,
                        Studies = p.Studies
                    };
                }
                catch (SqlException)
                {
                    tran.Rollback();
                }
                return res;
            }
        }

        public bool Login([FromBody] LoginRequest loginRequest)
        {
            using (var con = new SqlConnection(ConString))
            using (var com = new SqlCommand())
            {
                com.Connection = con;
                con.Open();
                try
                {
                    com.Parameters.AddWithValue("Index", loginRequest.Login);
                    com.CommandText = "SELECT Password, Salt FROM Student WHERE IndexNumber = @Index";

                    var dr = com.ExecuteReader();
                    dr.Read();
                    return PasswordEncryptor.Validate(loginRequest.Password, dr["Salt"].ToString(), dr["Password"].ToString());
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

        public void UpdateRefreshToken(string token, string login)
        {
            using (var connection = new SqlConnection(ConString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();
                try
                {
                    command.Parameters.AddWithValue("RT", token);
                    command.Parameters.AddWithValue("Index", login);
                    command.CommandText = "UPDATE Student SET RefreshToken = @RT WHERE IndexNumber = @Index";
                    command.ExecuteNonQuery();
                }
                catch (SqlException) { }
            }
        }

        public bool GetRefreshToken(string token)
        {
            using (var connection = new SqlConnection(ConString))
            using (var command = new SqlCommand())
            {
                command.Connection = connection;
                connection.Open();
                try
                {
                    command.Parameters.AddWithValue("RT", token);
                    command.CommandText = "SELECT * FROM Student WHERE RefreshToken = @RT";
                    return command.ExecuteReader().HasRows;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

    }
}