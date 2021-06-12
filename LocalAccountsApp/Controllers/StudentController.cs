using LocalAccountsApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Linq;

namespace LocalAccountsApp.Controllers
{
    public class StudentController : Controller
    {

        // GET: Student
        /*public ActionResult Index()
        {
            IEnumerable<StudentViewModel> students = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44305/api/");
                //HTTP GET
                var responseTask = client.GetAsync("School");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<StudentViewModel>>();
                    readTask.Wait();

                    students = readTask.Result;
                }
                else //web api sent error response 
                {
                    //log response status here..

                    students = Enumerable.Empty<StudentViewModel>();

                    //ModelState.AddModelError(string.Empty, "Server error. Please contact administrator.");
                }
            }
            return View(students);
        }*/ 

        public async Task<ActionResult> GetToken()
        {
            JObject tokenBased = new JObject();
            
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri("https://localhost:44305/");
                string body = "grant_type=password&username=daniel@email&password=Password1!";
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "/Token");
                request.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                var responseMessage = await client.SendAsync(request);
                if(responseMessage.IsSuccessStatusCode)
                {
                    var resultMessage = responseMessage.Content.ReadAsStringAsync().Result;
                    tokenBased = JsonConvert.DeserializeObject<JObject>(resultMessage);
                    Session["TokenNumber"] = tokenBased["access_token"].ToString();
                }
            }
            return Content(tokenBased["access_token"].ToString());
        }
        
        public async Task<ActionResult> Index()
        {
            JArray tokenBased = new JArray();
            IEnumerable<StudentGradeViewModel> studentGrades = null;


            using (var client = new HttpClient())
            {               
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri("https://localhost:44305/api/");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["TokenNumber"].ToString());              
                var responseMessage = await client.GetAsync("School/");

                if (responseMessage.IsSuccessStatusCode)
                {
                    var readTask = await responseMessage.Content.ReadAsAsync<IList<StudentGradeViewModel>>();
                    studentGrades = readTask;
                }
            }
            return View(studentGrades);
        }

        [HttpGet]
        public async Task<ActionResult> GetStudentById()
        {
            JArray tokenBased = new JArray();
            IList<StudentGradeViewModel> studentGrades = null;


            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri("https://localhost:44305/api/");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["TokenNumber"].ToString());
                var responseMessage = await client.GetAsync("School/GetStudent?studentId=52&courseId=1061");

                if (responseMessage.IsSuccessStatusCode)
                {
                    var readTask = await responseMessage.Content.ReadAsAsync<IList<StudentGradeViewModel>>();
                    studentGrades = readTask;
                }
            }
            return View(studentGrades);
        }

        [HttpGet]
        public async Task<ActionResult> GetAllStudent()
        {
            JArray tokenBased = new JArray();
            IList<StudentViewModel> studentGrades = null;


            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri("https://localhost:44305/api/");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["TokenNumber"].ToString());
                var responseMessage = await client.GetAsync("School/GetAllRegisteredStudents");

                if (responseMessage.IsSuccessStatusCode)
                {
                    var readTask = await responseMessage.Content.ReadAsAsync<IList<StudentViewModel>>();
                    studentGrades = readTask;
                }
            }
            return View(studentGrades);
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult CreateNewGrade()
        {
            return View();
        }

        [HttpPost]
        public ActionResult create(StudentViewModel student)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44305/api/");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<StudentViewModel>("School", student);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(student);
        }

        [HttpPost]
        public ActionResult CreateNewGrade(StudentGradeViewModel student)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44305/api/");

                //HTTP POST
                var postTask = client.PostAsJsonAsync<StudentGradeViewModel>("School/NewGrade", student);
                postTask.Wait();

                var result = postTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
            }

            ModelState.AddModelError(string.Empty, "Server Error. Please contact administrator.");

            return View(student);
        }

        public ActionResult Edit(int id, int courseId)
        {
            StudentGradeViewModel student = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44305/api/");
                //HTTP GET
                var responseTask = client.GetAsync("School?studentId=" + id.ToString() + "&courseId=" + courseId.ToString());
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<StudentGradeViewModel>();
                    readTask.Wait();

                    student = readTask.Result;
                }
            }

            return View(student);
        }

        public ActionResult AddToCourse(int id, int courseId)
        {
            StudentGradeViewModel student = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44305/api/");
                //HTTP GET
                var responseTask = client.GetAsync("School?studentId=" + id.ToString() + "&courseId=" + courseId.ToString());
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<StudentGradeViewModel>();
                    readTask.Wait();

                    student = readTask.Result;
                }
            }

            return View(student);
        }

        [HttpPost]
        public ActionResult Edit(StudentGradeViewModel student)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:44305/api/");

                //HTTP POST
                var putTask = client.PutAsJsonAsync<StudentGradeViewModel>("School/", student);
                putTask.Wait();

                var result = putTask.Result;
                if (result.IsSuccessStatusCode)
                {

                    return RedirectToAction("Index");
                }
            }
            return View(student);
        }

        /*public async Task<ActionResult> GetStudent()
        {
            JArray tokenBased = new JArray();
            IEnumerable<StudentGradeViewModel> studentGrades = null;


            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri("https://localhost:44305/api/");
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Session["TokenNumber"].ToString());
                var responseMessage = await client.GetAsync("School/");

                if (responseMessage.IsSuccessStatusCode)
                {
                    var readTask = await responseMessage.Content.ReadAsAsync<IList<StudentGradeViewModel>>();
                    studentGrades = readTask;
                }
            }
            return View(studentGrades);
        }*/
    }
}