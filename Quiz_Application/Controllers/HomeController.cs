using System.Data.SqlClient;
using System.Data;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Quiz_Application.Models;
using QuizApplication.Models;
using Microsoft.Extensions.Configuration;

namespace Quiz_Application.Controllers
{
    [CheckAccess]

    public class HomeController : Controller
    {
        //private readonly ILogger<HomeController> _logger;
        private IConfiguration Configuration;

        public HomeController(IConfiguration _configuration)
        {
            //_logger = logger;
            Configuration = _configuration;

        }

        public IActionResult Index()
        {
            DashboardViewModel dashboardData = new DashboardViewModel();
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            string connectionString = this.Configuration.GetConnectionString("ConnectionString");
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // Get Total Quizzes
                using (SqlCommand cmd = new SqlCommand("GetTotalQuizzes", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    dashboardData.TotalQuizzes = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Get Total Questions
                using (SqlCommand cmd = new SqlCommand("GetTotalQuestions", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    dashboardData.TotalQuestions = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Get Total Question Levels
                using (SqlCommand cmd = new SqlCommand("Sp_GetTotalLevels", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    dashboardData.TotalQuestionLevels = Convert.ToInt32(cmd.ExecuteScalar());
                }

                // Get Recent 10 Quizzes
                using (SqlCommand cmd = new SqlCommand("Sp_GetRecentQuizzes", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        dashboardData.RecentQuizzes.Add(new QuizModel
                        {
                            QuizID = Convert.ToInt32(reader["QuizID"]),
                            QuizName = reader["QuizName"].ToString(),
                            TotalQuestions = Convert.ToInt32(reader["TotalQuestions"]),
                            QuizDate = reader["QuizDate"].ToString()
                        });
                    }
                    reader.Close();
                }

                // Get Recent 10 Questions
                using (SqlCommand cmd = new SqlCommand("Sp_GetRecentQuestions", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        dashboardData.RecentQuestions.Add(new QuestionModel
                        {
                            QuestionID = Convert.ToInt32(reader["QuestionID"]),
                            QuestionText = reader["QuestionText"].ToString(),
                            QuestionMarks = Convert.ToInt32(reader["QuestionMarks"]),
                            CorrectOption = reader["CorrectOption"].ToString()
                        });
                    }
                    reader.Close();
                }
            }

            return View(dashboardData);
        }
        

        public IActionResult Privacy()
        {
            return View();
        }
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
        public IActionResult Contect()
        {
            return View();
        }
        public IActionResult Error404()
        {
            return View();
        }
        public IActionResult TagHelperDemo()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
