using LocalAccountsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace LocalAccountsApp.Controllers
{
    public class SchoolController : ApiController
    {
        [Authorize]
        public IHttpActionResult GetAllStudents()
        {

            IList<StudentGradeViewModel> students = null;

            using (var ctx = new SchoolDBEntities())
            {
                students = ctx.StudentGrades.Include("Course").Include("Person").
                    Select(s => new StudentGradeViewModel()
                    {
                        CourseID = s.CourseID,
                        StudentID = s.StudentID,
                        Grade = s.Grade,
                        Student = new StudentViewModel()
                        {
                            FirstName = s.Person.FirstName,
                            LastName = s.Person.LastName
                        },
                        Course = new CourseViewModel()
                        {
                            CourseID = s.Course.CourseID,
                            Title = s.Course.Title
                        }
                    }).ToList<StudentGradeViewModel>();
            }

            if (students.Count == 0)
            {
                return NotFound();
            }

            return Ok(students);
        }

        [HttpGet]
        public IHttpActionResult GetStudent(int studentId, int courseId)
        {
            StudentGradeViewModel studentsGrade = null;

            using (var ctx = new SchoolDBEntities())
            {
                studentsGrade = ctx.StudentGrades.Include("Course").Include("Person").
                    Where(s => s.StudentID == studentId && s.CourseID == courseId).
                    Select(s => new StudentGradeViewModel() 
                    { 
                        CourseID = s.CourseID, 
                        StudentID = s.StudentID, 
                        Grade = s.Grade,
                        Student = new StudentViewModel()
                        {
                            Id = s.Person.PersonID,
                            FirstName = s.Person.FirstName,
                            LastName = s.Person.LastName
                        },
                        Course = new CourseViewModel()
                        {
                            CourseID = s.Course.CourseID,
                            Title = s.Course.Title
                        }
                    }).FirstOrDefault();
               
            }

            List <decimal> res = new List<decimal>();

            

            if (studentsGrade == null)
            {
                return NotFound();
            }

            return Ok(studentsGrade);


        }

        [HttpGet]
        public IHttpActionResult GetStudent(int studentId)
        {
            StudentViewModel student = null;

            using (var ctx = new SchoolDBEntities())
            {
                student = ctx.People.Where(s => s.PersonID == studentId).
                    Select(s => new StudentViewModel()
                    {
                        Id = s.PersonID,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        Discriminator = s.Discriminator
                    }).
                    FirstOrDefault();

            }

            List<decimal> res = new List<decimal>();



            if (student == null)
            {
                return NotFound();
            }

            return Ok(student);


        }

        [HttpGet]
        [Route("api/School/GetAllRegisteredStudents")]
        public IHttpActionResult GetAllRegisteredStudents()
        {
            List<StudentViewModel> studentsGrade = null;

            using (var ctx = new SchoolDBEntities())
            {
                studentsGrade = ctx.People.
                    Select(s => new StudentViewModel()
                    {
                        Id = s.PersonID,
                        FirstName = s.FirstName,
                        LastName = s.LastName,
                        Discriminator = s.Discriminator
                    }).ToList<StudentViewModel>();

            }

            List<decimal> res = new List<decimal>();



            if (studentsGrade == null)
            {
                return NotFound();
            }

            return Ok(studentsGrade);


        }

        [HttpPost]
        public IHttpActionResult PostNewStudent(StudentViewModel student)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            using (var context = new SchoolDBEntities())
            {
                context.People.Add(new Person()
                {
                    PersonID = student.Id,
                    FirstName = student.FirstName,
                    LastName = student.LastName,
                    Discriminator = student.Discriminator
                });

                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPost]
        [Route("api/School/NewGrade")]
        public IHttpActionResult PostNewGrade(StudentGradeViewModel student)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            using (var context = new SchoolDBEntities())
            {
                context.StudentGrades.Add(new StudentGrade()
                {
                    CourseID = student.CourseID,
                    StudentID = student.StudentID,
                    Grade = student.Grade
                });

                context.SaveChanges();
            }

            return Ok();
        }

        [HttpPut]
        public IHttpActionResult Put(StudentGradeViewModel student)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            using (var ctx = new SchoolDBEntities())
            {
                var studentsGrade = ctx.StudentGrades.Include("Course").Include("Person").
                    Where(s => s.StudentID == student.StudentID && s.CourseID == student.CourseID).
                    FirstOrDefault();

                if (studentsGrade != null)
                {
                    studentsGrade.Grade = student.Grade;

                    ctx.SaveChanges();
                }
                else
                {
                    return NotFound();
                }
            }

            return Ok();
        }

        [HttpPut]
        [Route("api/School/UpdateStudent")]
        public IHttpActionResult Put(StudentViewModel student)
        {
            if (!ModelState.IsValid)
                return BadRequest("Not a valid model");

            using (var ctx = new SchoolDBEntities())
            {
                var studentsGrade = ctx.People.
                    Where(s => s.PersonID == student.Id).
                    FirstOrDefault();

                if (studentsGrade != null)
                {
                    studentsGrade.FirstName = student.FirstName;
                    studentsGrade.LastName = student.LastName;
                    studentsGrade.Discriminator = student.Discriminator;

                    ctx.SaveChanges();
                }
                else
                {
                    return NotFound();
                }
            }

            return Ok();
        }

        public IHttpActionResult Delete(int id)
        {
            if (id <= 0)
                return BadRequest("Not a valid student id");

            using (var ctx = new SchoolDBEntities())
            {
                var student = ctx.People
                    .Where(s => s.PersonID == id)
                    .FirstOrDefault();

                var grades = ctx.StudentGrades
                    .Where(s => s.StudentID == id).ToList();

                foreach (var grade in grades)
                {
                    ctx.StudentGrades.Remove(grade);
                }
                ctx.SaveChanges();

                ctx.Entry(student).State = System.Data.Entity.EntityState.Deleted;
                ctx.SaveChanges();
            }

            return Ok();
        }
    }
}
