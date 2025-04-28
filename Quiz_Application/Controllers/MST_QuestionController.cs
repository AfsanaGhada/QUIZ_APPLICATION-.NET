using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Quiz_Application.Models;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;

namespace Quiz_Application.Controllers
{
    public class MST_QuestionController : Controller
    {
        private IConfiguration configuration;

        public MST_QuestionController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        //[Route("AddEditQuestion")]
        //public IActionResult QuestionForm()
        //{
        //    return View();
        //}
        public IActionResult QuestionList()
        {
            QuestionLevelDropdown();
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Sp_MST_Question_SelectAllQuestions";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult QuestionDelete(int QuestionID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Sp_MST_Question_DeleteQuestionByID";
                    command.Parameters.Add("@QuestionID", SqlDbType.Int).Value = QuestionID;


                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "Question deleted successfully.";
                return RedirectToAction("QuestionList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the Qustion: " + ex.Message;
                return RedirectToAction("QuestionList");
            }
        }


        //public IActionResult questionAddEdit(Question model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        string connectionString = configuration.GetConnectionString("ConnectionString");
        //        SqlConnection connection = new SqlConnection(connectionString);
        //        connection.Open();
        //        SqlCommand command = connection.CreateCommand();
        //        command.CommandType = CommandType.StoredProcedure;

        //        if (model.QuestionID == 0)
        //        {
        //            command.CommandText = "Sp_MST_Question_InsertNewQuestion";
        //        }
        //        else
        //        {
        //            command.CommandText = "Sp_MST_Question_UpdateQuestion";
        //            command.Parameters.Add("@QuestionID", SqlDbType.Int).Value = model.QuestionID;
        //        }
        //        command.Parameters.Add("@QuestionText", SqlDbType.VarChar).Value = model.QuestionText;
        //        command.Parameters.Add("@QuestionLevelID", SqlDbType.VarChar).Value = model.QuestionLevelID;
        //        command.Parameters.Add("@OptionA", SqlDbType.VarChar).Value = model.@OptionA;
        //        command.Parameters.Add("@OptionB", SqlDbType.VarChar).Value = model.@OptionB;
        //        command.Parameters.Add("@OptionC", SqlDbType.VarChar).Value = model.@OptionC;
        //        command.Parameters.Add("@OptionD", SqlDbType.VarChar).Value = model.@OptionD;

        //        command.Parameters.Add("@CorrectOption", SqlDbType.VarChar).Value = model.CorrectOption;
              

        //        //command.Parameters.Add("@UserID", SqlDbType.Int).Value = CommonVariable.UserID();
        //        command.ExecuteNonQuery();
        //        return RedirectToAction("QuestionList");
        //    }

        //    return View("QuestionForm", model);
        //}


        public IActionResult QuestionForm(int QuestionID)
        {
            if (QuestionID == 0)
            {
                TempData["PageTitle"] = "Add Question";
            }
            else
            {
                TempData["PageTitle"] = "Edit Question";
            }

            //------------Question Level Drop Down Model
            string questionLevelConnectionString = this.configuration.GetConnectionString("ConnectionString");

            FillQuestionLevelDropDown();


            ViewBag.QuestionID = QuestionID;
            SqlConnection QuestionSqlConnection = new SqlConnection(questionLevelConnectionString);
            QuestionSqlConnection.Open();
            SqlCommand questionSqlCommand = QuestionSqlConnection.CreateCommand();
            questionSqlCommand.CommandType = CommandType.StoredProcedure;
            questionSqlCommand.CommandText = "Sp_MST_Question_SelectQuestionByID";
            questionSqlCommand.Parameters.AddWithValue("@QuestionID", QuestionID);
            SqlDataReader questionReader = questionSqlCommand.ExecuteReader();
            DataTable questionTable = new DataTable();
            questionTable.Load(questionReader);
            Question questionModel = new Question();

            foreach (DataRow dataRow in questionTable.Rows)
            {
                questionModel.QuestionID = Convert.ToInt32(@dataRow["QuestionID"]);
                questionModel.QuestionText = @dataRow["QuestionText"].ToString();
                questionModel.QuestionLevelID = Convert.ToInt32(@dataRow["QuestionLevelID"]);
                questionModel.OptionA = @dataRow["OptionA"].ToString();
                questionModel.OptionB = @dataRow["OptionB"].ToString();
                questionModel.OptionC = @dataRow["OptionC"].ToString();
                questionModel.OptionD = @dataRow["OptionD"].ToString();
                questionModel.CorrectOption = @dataRow["CorrectOption"].ToString();
                questionModel.QuestionMarks = Convert.ToInt32(@dataRow["QuestionMarks"]);
                questionModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
            }
            Console.WriteLine(questionModel);
            return View("QuestionForm", questionModel);
        }

        private void FillQuestionLevelDropDown()
        {
            string questionLevelConnectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection questionLevelSqlConnection = new SqlConnection(questionLevelConnectionString);
            questionLevelSqlConnection.Open();
            SqlCommand questionLevelSqlCommand = questionLevelSqlConnection.CreateCommand();
            questionLevelSqlCommand.CommandType = CommandType.StoredProcedure;
            questionLevelSqlCommand.CommandText = "Sp_MST_QuestionLevel_SelectAll";
            SqlDataReader questionLevelReader = questionLevelSqlCommand.ExecuteReader();
            DataTable questionLevelDataTable = new DataTable();
            questionLevelDataTable.Load(questionLevelReader);
            List<QuestionLevelDropdown> questionLevelList = new List<QuestionLevelDropdown>();
            foreach (DataRow data in questionLevelDataTable.Rows)
            {
                QuestionLevelDropdown questionLevelDropDownModel = new QuestionLevelDropdown();
                questionLevelDropDownModel.QuestionLevelID = Convert.ToInt32(data["QuestionLevelID"]);
                questionLevelDropDownModel.QuestionLevel = data["QuestionLevel"].ToString();
                questionLevelList.Add(questionLevelDropDownModel);
            }
            ViewBag.QuestionLevelList = questionLevelList;
        }

        public IActionResult QuestionAddEdit(int? QuestionID)
        {
            Question model = new Question();

            if (QuestionID != null && QuestionID > 0)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Sp_MST_Question_SelectQuestionByID", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@QuestionID", SqlDbType.Int).Value = QuestionID;

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        model.QuestionID = Convert.ToInt32(reader["QuestionID"]);
                        model.QuestionText = reader["QuestionText"].ToString();
                        model.QuestionLevelID = Convert.ToInt32(reader["QuestionLevelID"]);
                        model.OptionA = reader["OptionA"].ToString();
                        model.OptionB = reader["OptionB"].ToString();
                        model.OptionC = reader["OptionC"].ToString();
                        model.OptionD = reader["OptionD"].ToString();
                        model.CorrectOption = reader["CorrectOption"].ToString();
                        model.QuestionMarks = Convert.ToInt32(reader["QuestionMarks"]);
                        model.IsActive = Convert.ToBoolean(reader["IsActive"]);
                        model.UserID = Convert.ToInt32(reader["UserID"]);
                    }
                }
            }
            return View("QuestionForm", model);
        }

        // POST: Save Question (Insert/Update)
        [HttpPost]
        public IActionResult QuestionSave(Question model)
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

                    if (model.QuestionID == null || model.QuestionID == 0) // Insert
                    {
                        command.CommandText = "Sp_MST_Question_InsertNewQuestion";
                    }
                    else // Update
                    {
                        command.CommandText = "Sp_MST_Question_UpdateQuestion";
                        command.Parameters.Add("@QuestionID", SqlDbType.Int).Value = model.QuestionID;
                    }

                    // Common parameters
                    command.Parameters.Add("@QuestionText", SqlDbType.NVarChar).Value = model.QuestionText;
                    command.Parameters.Add("@QuestionLevelID", SqlDbType.Int).Value = model.QuestionLevelID;
                    command.Parameters.Add("@OptionA", SqlDbType.NVarChar).Value = model.OptionA;
                    command.Parameters.Add("@OptionB", SqlDbType.NVarChar).Value = model.OptionB;
                    command.Parameters.Add("@OptionC", SqlDbType.NVarChar).Value = model.OptionC;
                    command.Parameters.Add("@OptionD", SqlDbType.NVarChar).Value = model.OptionD;
                    command.Parameters.Add("@CorrectOption", SqlDbType.NVarChar).Value = model.CorrectOption;
                    command.Parameters.Add("@QuestionMarks", SqlDbType.Int).Value = model.QuestionMarks;
                    command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = model.IsActive;
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = model.UserID;

                    command.ExecuteNonQuery();
                }
                return RedirectToAction("QuestionList");
            }

            return View("QuestionForm", model);
        }

        public IActionResult QuestionSearch(int? questionLevel, string questionText, int? questionMarks)
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("Sp_MST_Question_Search", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@QuestionLevel", (object)questionLevel ?? DBNull.Value);
                command.Parameters.AddWithValue("@QuestionText", (object)questionText ?? DBNull.Value);
                command.Parameters.AddWithValue("@QuestionMarks", (object)questionMarks ?? DBNull.Value);

                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);

                //  Debug columns
                foreach (DataColumn col in table.Columns)
                    Console.WriteLine(col.ColumnName);

                return View("QuestionList", table);
            }
        }

        public void QuestionLevelDropdown()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[Sp_MST_QuestionLevel_SelectfromDropdown]";
            SqlDataReader reader = command.ExecuteReader();
            List<QuestionLevel> questionLevels = new List<QuestionLevel>();
            while(reader.Read())
            {
                if (reader.HasRows)
                {
                    QuestionLevel q = new QuestionLevel();
                    q.QuestionLevelID = Convert.ToInt32(reader["QuestionLevelID"]);
                    q.QuestionLevelName = reader["QuestionLevel"].ToString();

                    questionLevels.Add(q);
                }
            }
            ViewBag.QuestionLevels = questionLevels;
        }
        public IActionResult ExportToExcel()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("Sp_MST_Question_SelectAllQuestions", connection) // Updated stored procedure
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataReader reader = command.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Questions");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "QuestionID";
                    worksheet.Cells[1, 2].Value = "QuestionText";
                    worksheet.Cells[1, 3].Value = "QuestionLevelID";
                    worksheet.Cells[1, 4].Value = "QuestionLevel";
                    worksheet.Cells[1, 5].Value = "OptionA";
                    worksheet.Cells[1, 6].Value = "OptionB";
                    worksheet.Cells[1, 7].Value = "OptionC";
                    worksheet.Cells[1, 8].Value = "OptionD";
                    worksheet.Cells[1, 9].Value = "CorrectOption";
                    worksheet.Cells[1, 10].Value = "QuestionMarks";
                    worksheet.Cells[1, 11].Value = "IsActive";
                    worksheet.Cells[1, 12].Value = "UserName";

                    // Add data
                    int row = 2;
                    foreach (DataRow item in data.Rows)
                    {
                        worksheet.Cells[row, 1].Value = item["QuestionID"];
                        worksheet.Cells[row, 2].Value = item["QuestionText"];
                        worksheet.Cells[row, 3].Value = item["QuestionLevelID"];
                        worksheet.Cells[row, 4].Value = item["QuestionLevel"];
                        worksheet.Cells[row, 5].Value = item["OptionA"];
                        worksheet.Cells[row, 6].Value = item["OptionB"];
                        worksheet.Cells[row, 7].Value = item["OptionC"];
                        worksheet.Cells[row, 8].Value = item["OptionD"];
                        worksheet.Cells[row, 9].Value = item["CorrectOption"];
                        worksheet.Cells[row, 10].Value = item["QuestionMarks"];
                        worksheet.Cells[row, 11].Value = Convert.ToBoolean(item["IsActive"]) ? "Yes" : "No";
                        worksheet.Cells[row, 12].Value = item["UserName"];
                        row++;
                    }

                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0;

                    string excelName = $"Questions-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                }
            }
        }


    }
}
