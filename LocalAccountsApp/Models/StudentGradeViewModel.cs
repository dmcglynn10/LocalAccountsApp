using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocalAccountsApp.Models
{
    public class StudentGradeViewModel
    {
        public int CourseID { get; set; }
        public int StudentID { get; set; }
        public Nullable<decimal> Grade { get; set; }

        public StudentViewModel Student { get; set; }

        public CourseViewModel Course { get; set; }
    }
}