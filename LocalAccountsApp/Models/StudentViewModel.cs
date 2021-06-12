using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LocalAccountsApp.Models
{
    public class StudentViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<StudentGrade> studentGrades { get; set; }
        
        public string Discriminator { get; set; }
    }
}