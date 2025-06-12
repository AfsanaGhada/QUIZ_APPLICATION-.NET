using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using Quiz_Application.Models;
using System.Data;
using System.Data.SqlClient;
namespace Quiz_Application.Controllers
{
    public class MST_QuizController : Controller
    {
        private IConfiguration configuration;

        public MST_QuizController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult QuizForm(int QuizID)
        {
            if (QuizID == 0)
            {
                TempData["PageTitle"] = "Add Quiz";
            }
            else
            {
                TempData["PageTitle"] = "Edit Quiz";
            }

            ViewBag.QuizID = QuizID;
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand SqlCommand = connection.CreateCommand();
            SqlCommand.CommandType = CommandType.StoredProcedure;
            SqlCommand.CommandText = "Sp_MST_Quiz_SelectQuizByID";
            SqlCommand.Parameters.AddWithValue("@QuizID", QuizID);
            SqlDataReader Reader = SqlCommand.ExecuteReader();
            DataTable Table = new DataTable();
            Table.Load(Reader);
            Quiz Model = new Quiz();

            foreach (DataRow dataRow in Table.Rows)
            {
                Model.UserID = Convert.ToInt32(@dataRow["UserID"]);
                Model.QuizName = @dataRow["QuizName"].ToString();
                Model.TotalQuestions = Convert.ToInt32(@dataRow["TotalQuestions"]);

                Model.QuizDate = Convert.ToDateTime(@dataRow["QuizDate"]);

            }
            Console.WriteLine(Model);
            return View("QuizForm", Model);
        }
        public IActionResult QuizList()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Sp_MST_Quiz_SelectAllQuizzes";
            //command.Parameters.AddWithValue("@UserId", CommonVariable.UserID());
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult QuizDelete(int QuizID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Sp_MST_Quiz_DeleteQuizByID";
                    command.Parameters.Add("@QuizID", SqlDbType.Int).Value = QuizID;


                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Quiz deleted successfully.";
                return RedirectToAction("QuizList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the Quiz: " + ex.Message;
                return RedirectToAction("QuizList");
            }
        }


        public IActionResult QuizAddEdit(int? QuizID)
        {
            Quiz model = new Quiz();

            if (QuizID != null && QuizID > 0)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Sp_MST_Quiz_SelectQuizByID", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@QuizID", SqlDbType.Int).Value = QuizID;

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        model.QuizID = Convert.ToInt32(reader["QuizID"]);
                        model.QuizName = reader["QuizName"].ToString();
                        model.TotalQuestions =Convert. ToInt32(reader["TotalQuestions"]);
                        model.QuizDate = Convert.ToDateTime(reader["QuizDate"]);
                        model.UserID = Convert.ToInt32(reader["UserID"]);
                    }
                }
            }
            return View("QuizForm", model);
        }

        // POST: Save Question (Insert/Update)
        [HttpPost]
        public IActionResult QuizSave(Quiz model)
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

                    if (model.QuizID == null || model.QuizID == 0) // Insert
                    {
                        command.CommandText = "Sp_MST_Quiz_InsertNewQuiz";
                    }
                    else // Update
                    {
                        command.CommandText = "Sp_MST_Quiz_UpdateQuiz";
                        command.Parameters.Add("@QuizID", SqlDbType.Int).Value = model.QuizID;
                    }

                    // Common parameters
                    command.Parameters.Add("@QuizName", SqlDbType.NVarChar).Value = model.QuizName;
                    command.Parameters.Add("@TotalQuestions", SqlDbType.Int).Value = model.TotalQuestions;
                    command.Parameters.Add("@QuizDate", SqlDbType.DateTime).Value = model.QuizDate;
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = model.UserID;

                    command.ExecuteNonQuery();
                }
                return RedirectToAction("QuizList");
            }

            return View("QuizForm", model);
        }
        public IActionResult ExportToExcel()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("Sp_MST_Quiz_SelectAllQuizzes", connection) // Updated stored procedure
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataReader reader = command.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Quizzes");

                    // Add headers (without UserID)
                    worksheet.Cells[1, 1].Value = "QuizID";
                    worksheet.Cells[1, 2].Value = "QuizName";
                    worksheet.Cells[1, 3].Value = "TotalQuestions";
                    worksheet.Cells[1, 4].Value = "QuizDate";
                    worksheet.Cells[1, 5].Value = "UserName";
                    worksheet.Cells[1, 6].Value = "Created";
                    worksheet.Cells[1, 7].Value = "Modified";

                    // Add data
                    int row = 2;
                    foreach (DataRow item in data.Rows)
                    {
                        worksheet.Cells[row, 1].Value = item["QuizID"];
                        worksheet.Cells[row, 2].Value = item["QuizName"];
                        worksheet.Cells[row, 3].Value = item["TotalQuestions"];
                        worksheet.Cells[row, 4].Value = Convert.ToDateTime(item["QuizDate"]).ToString("yyyy-MM-dd"); // Formatting date
                        worksheet.Cells[row, 5].Value = item["UserName"];
                        worksheet.Cells[row, 6].Value = Convert.ToDateTime(item["Created"]).ToString("yyyy-MM-dd HH:mm:ss"); // Formatting datetime
                        worksheet.Cells[row, 7].Value = Convert.ToDateTime(item["Modified"]).ToString("yyyy-MM-dd HH:mm:ss"); // Formatting datetime
                        row++;
                    }

                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0;

                    string excelName = $"Quizzes-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                }
            }
        }

        [HttpPost]
        public IActionResult QuizSearch(string quizName, int? minQuestion, int? maxQuestion, DateTime? quizDate)
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("Sp_MST_Quiz_Search", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@QuizName", (object)quizName ?? DBNull.Value);
                command.Parameters.AddWithValue("@MinQuestion", (object)minQuestion ?? DBNull.Value);
                command.Parameters.AddWithValue("@MaxQuestion", (object)maxQuestion ?? DBNull.Value);
                command.Parameters.AddWithValue("@QuizDate", (object)quizDate ?? DBNull.Value);

                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);

                return View("QuizList", table); // Ensure "QuizList" view exists
            }
        }
    }
}
