using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;

namespace Quiz_Application.Controllers
{
    public class MST_QuizWiseQuestionsController : Controller
    {
        private IConfiguration configuration;

        public MST_QuizWiseQuestionsController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult QuizWiseQuestionsForm()
        {
            return View();
        }
        public IActionResult QuizWiseQuestionsList()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Sp_MST_QuizWiseQuestions_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult QuizWiseQuestionsDelete(int QuizWiseQuestionsID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "PR_City_DeleteByPK";
                    command.Parameters.Add("@QuizWiseQuestionsID", SqlDbType.Int).Value = QuizWiseQuestionsID;


                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "QuizWiseQuestions deleted successfully.";
                return RedirectToAction("QuizWiseQuestionsList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the QuizWiseQuestions: " + ex.Message;
                return RedirectToAction("QuizWiseQuestionsList");
            }
        }
    }
    
}
