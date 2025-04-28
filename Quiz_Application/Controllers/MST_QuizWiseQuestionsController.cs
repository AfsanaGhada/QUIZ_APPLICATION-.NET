using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Quiz_Application.Models;
using OfficeOpenXml;

namespace Quiz_Application.Controllers
{
    public class MST_QuizWiseQuestionsController : Controller
    {
        private IConfiguration configuration;

        public MST_QuizWiseQuestionsController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult QuizWiseQuestionsForm(int QuizWiseQuestionsID)
        {
            FillQuizDropDown();
            FillQuestionDropDown();
            if (QuizWiseQuestionsID == 0)
            {
                TempData["PageTitle"] = "Add QuizWiseQuestion";
            }
            else
            {
                TempData["PageTitle"] = "Edit QuizWiseQuestion";
            }

            //------------Question Level Drop Down Model
            string questionLevelConnectionString = this.configuration.GetConnectionString("ConnectionString");




            ViewBag.QuizWiseQuestionsID = QuizWiseQuestionsID;
            SqlConnection QuestionSqlConnection = new SqlConnection(questionLevelConnectionString);
            QuestionSqlConnection.Open();
            SqlCommand questionSqlCommand = QuestionSqlConnection.CreateCommand();
            questionSqlCommand.CommandType = CommandType.StoredProcedure;
            questionSqlCommand.CommandText = "Sp_MST_QuizWiseQuestions_SelectByID";
            questionSqlCommand.Parameters.AddWithValue("@QuizWiseQuestionsID", QuizWiseQuestionsID);
            SqlDataReader questionReader = questionSqlCommand.ExecuteReader();
            DataTable questionTable = new DataTable();
            questionTable.Load(questionReader);
            QuizWiseQuestion questionModel = new QuizWiseQuestion();

            foreach (DataRow dataRow in questionTable.Rows)
            {
                questionModel.QuizWiseQuestionsID = Convert.ToInt32(@dataRow["QuizWiseQuestionsID"]);
                questionModel.QuizID = Convert.ToInt32(@dataRow["QuizID"]);
                questionModel.QuestionID = Convert.ToInt32(@dataRow["QuestionID"]);
                questionModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
            }
            Console.WriteLine(questionModel);
            return View("QuizWiseQuestionsForm", questionModel);
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
                    command.CommandText = "Sp_MST_QuizWiseQuestions_DeleteByID";
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
        public IActionResult QuizWiseQuestionAddEdit(int? QuizWiseQuestionsID)
        {
            QuizWiseQuestion model = new QuizWiseQuestion();

            if (QuizWiseQuestionsID != null && QuizWiseQuestionsID > 0)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Sp_MST_QuizWiseQuestions_SelectByID", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@QuizWiseQuestionsID", SqlDbType.Int).Value = QuizWiseQuestionsID;

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        model.QuizWiseQuestionsID = Convert.ToInt32(reader["QuizWiseQuestionsID"]);
                        model.QuizID = Convert.ToInt32(reader["QuizID"]);
                        model.QuestionID = Convert.ToInt32(reader["QuestionID"]);
                        model.UserID = Convert.ToInt32(reader["UserID"]);
                    }
                }
            }
            return View("QuizWiseQuestionsForm", model);
        }

        // POST: Save Question (Insert/Update)
        [HttpPost]
        public IActionResult QuizWiseQuestionSave(QuizWiseQuestion model)
        {
            if (ModelState.IsValid)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand
                    {
                        Connection = connection,
                        CommandType = CommandType.StoredProcedure
                    };

                    if (model.QuizWiseQuestionsID == null || model.QuizWiseQuestionsID == 0) // Insert
                    {
                        command.CommandText = "Sp_MST_QuizWiseQuestions_Insert";
                    }
                    else // Update
                    {
                        command.CommandText = "Sp_MST_QuizWiseQuestions_Update";
                        command.Parameters.Add("@QuizWiseQuestionsID", SqlDbType.Int).Value = model.QuizWiseQuestionsID;
                    }

                    // Common parameters
                    
                    command.Parameters.Add("@QuizID", SqlDbType.Int).Value = model.QuizID;
                    command.Parameters.Add("@QuestionID", SqlDbType.Int).Value = model.QuestionID;
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = model.UserID;


                    command.ExecuteNonQuery();
                }
                return RedirectToAction("QuizWiseQuestionsList");
            }

            return View("QuizWiseQuestionsForm", model);
        }
        public IActionResult ExportToExcel()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("Sp_MST_QuizWiseQuestions_SelectAll", connection) // Updated stored procedure
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataReader reader = command.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("QuizWiseQuestions");

                    // Add headers (without UserID)
                    worksheet.Cells[1, 1].Value = "QuizWiseQuestionsID";
                    worksheet.Cells[1, 2].Value = "QuizID";
                    worksheet.Cells[1, 3].Value = "QuizName";
                    worksheet.Cells[1, 4].Value = "QuestionID";
                    worksheet.Cells[1, 5].Value = "QuestionText";
                    worksheet.Cells[1, 6].Value = "UserName"; // Kept UserName but removed UserID
                    worksheet.Cells[1, 7].Value = "Created";
                    worksheet.Cells[1, 8].Value = "Modified";

                    // Add data
                    int row = 2;
                    foreach (DataRow item in data.Rows)
                    {
                        worksheet.Cells[row, 1].Value = item["QuizWiseQuestionsID"];
                        worksheet.Cells[row, 2].Value = item["QuizID"];
                        worksheet.Cells[row, 3].Value = item["QuizName"];
                        worksheet.Cells[row, 4].Value = item["QuestionID"];
                        worksheet.Cells[row, 5].Value = item["QuestionText"];
                        worksheet.Cells[row, 6].Value = item["UserName"]; // Fetching UserName instead of UserID
                        worksheet.Cells[row, 7].Value = Convert.ToDateTime(item["Created"]).ToString("yyyy-MM-dd HH:mm:ss"); // Formatting datetime
                        worksheet.Cells[row, 8].Value = Convert.ToDateTime(item["Modified"]).ToString("yyyy-MM-dd HH:mm:ss"); // Formatting datetime
                        row++;
                    }

                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0;

                    string excelName = $"QuizWiseQuestions-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                }
            }
        }
        private void FillQuizDropDown()
        {
            string quizConnectionString = this.configuration.GetConnectionString("ConnectionString");
            using (SqlConnection quizSqlConnection = new SqlConnection(quizConnectionString))
            {
                quizSqlConnection.Open();
                using (SqlCommand quizSqlCommand = quizSqlConnection.CreateCommand())
                {
                    quizSqlCommand.CommandType = CommandType.StoredProcedure;
                    quizSqlCommand.CommandText = "Sp_MST_Quiz_SelectAllQuizzes";
                    using (SqlDataReader quizReader = quizSqlCommand.ExecuteReader())
                    {
                        List<QuizDropdown> quizList = new List<QuizDropdown>();
                        while (quizReader.Read())
                        {
                            quizList.Add(new QuizDropdown
                            {
                                QuizID = quizReader.GetInt32(0), // QuizID
                                QuizName = quizReader.GetString(1) // QuizName
                            });
                        }
                        ViewBag.QuizList = quizList;
                    }
                }
            }
        }
        private void FillQuestionDropDown()
        {
            string questionConnectionString = this.configuration.GetConnectionString("ConnectionString");
            using (SqlConnection questionSqlConnection = new SqlConnection(questionConnectionString))
            {
                questionSqlConnection.Open();
                using (SqlCommand questionSqlCommand = questionSqlConnection.CreateCommand())
                {
                    questionSqlCommand.CommandType = CommandType.StoredProcedure;
                    questionSqlCommand.CommandText = "Sp_MST_Question_SelectAllQuestions";
                    using (SqlDataReader questionReader = questionSqlCommand.ExecuteReader())
                    {
                        List<QuestionDropdown> questionList = new List<QuestionDropdown>();
                        while (questionReader.Read())
                        {
                            questionList.Add(new QuestionDropdown
                            {
                                QuestionID = questionReader.GetInt32(0), // QuestionID
                                QuestionText = questionReader.GetString(1) // QuestionText
                            });
                        }

                        if (questionList.Count == 0) // Debugging
                        {
                            throw new Exception("No questions found from stored procedure.");
                        }

                        ViewBag.QuestionList = questionList;
                    }
                }
            }
        }
        public IActionResult QuizWiseQuestionSearch(string quizName, string question, int? quizWiseQuestionID)
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                SqlCommand command = new SqlCommand("Sp_MST_QuizWiseQuestion_Search", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@QuizName", (object)quizName ?? DBNull.Value);
                command.Parameters.AddWithValue("@Question", (object)question ?? DBNull.Value);
                command.Parameters.AddWithValue("@QuizWiseQuestionID", (object)quizWiseQuestionID ?? DBNull.Value);

                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);

                return View("QuizWiseQuestionsList", table); // Ensure this view exists in Views/QuizWiseQuestions/
            }
        }




    }

}
