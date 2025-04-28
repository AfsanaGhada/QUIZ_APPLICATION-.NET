using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Quiz_Application.Models;
using OfficeOpenXml;
using System.Reflection.Emit;

namespace Quiz_Application.Controllers
{
    public class MST_QuestionLevelController : Controller
    {
        private IConfiguration configuration;

        public MST_QuestionLevelController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult QuestionLevelForm(int QuestionLevelID)
        {

            if (QuestionLevelID == 0)
            {
                TempData["PageTitle"] = "Add QuestionLevel";
            }
            else
            {
                TempData["PageTitle"] = "Edit QuestionLevel";
            }

            //------------Question Level Drop Down Model
            string questionLevelConnectionString = this.configuration.GetConnectionString("ConnectionString");




            ViewBag.QuestionLevelID = QuestionLevelID;
            SqlConnection QuestionSqlConnection = new SqlConnection(questionLevelConnectionString);
            QuestionSqlConnection.Open();
            SqlCommand questionSqlCommand = QuestionSqlConnection.CreateCommand();
            questionSqlCommand.CommandType = CommandType.StoredProcedure;
            questionSqlCommand.CommandText = "Sp_MST_QuestionLevel_SelectByID";
            questionSqlCommand.Parameters.AddWithValue("@QuestionLevelID", QuestionLevelID);
            SqlDataReader questionlevelReader = questionSqlCommand.ExecuteReader();
            DataTable questionlevelTable = new DataTable();
            questionlevelTable.Load(questionlevelReader);
            QuestionLevel questionLevelModel = new QuestionLevel();

            foreach (DataRow dataRow in questionlevelTable.Rows)
            {
                questionLevelModel.QuestionLevelID = Convert.ToInt32(@dataRow["QuestionLevelID"]);
                questionLevelModel.QuestionLevelName = @dataRow["QuestionLevel"].ToString();
                questionLevelModel.UserID = Convert.ToInt32(@dataRow["UserID"]);
            }
            Console.WriteLine(questionLevelModel);
            return View("QuestionLevelForm", questionLevelModel);
        }
        public IActionResult QuestionLevelList()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Sp_MST_QuestionLevel_SelectAll";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult QuestionLevelDelete(int QuestionLevelID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Sp_MST_QuestionLevel_DeleteByID";
                    command.Parameters.Add("@QuestionLevelID", SqlDbType.Int).Value = QuestionLevelID;


                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "QuestionLevel deleted successfully.";
                return RedirectToAction("QuestionLevelList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the QuestionLevel: " + ex.Message;
                return RedirectToAction("QuestionLevelList");
            }
        }
        public IActionResult QuestionLevelAddEdit(int? QuestionLevelID)
        {
            QuestionLevel model = new QuestionLevel();

            if (QuestionLevelID != null && QuestionLevelID > 0)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Sp_MST_QuestionLevel_SelectByID", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@QuestionLevelID", SqlDbType.Int).Value = QuestionLevelID;

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        model.QuestionLevelID = Convert.ToInt32(reader["QuestionLevelID"]);
                        model.QuestionLevelName = reader["QuestionLevel"].ToString();
                        model.UserID = Convert.ToInt32(reader["UserID"]);
                       
                    }
                }
            }
            return View("QuestionLevelForm", model);
        }

        // POST: Save Question (Insert/Update)
        [HttpPost]
        public IActionResult QuestionLevelSave(QuestionLevel model)
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

                    if (model.QuestionLevelID == null || model.QuestionLevelID == 0) // Insert
                    {
                        command.CommandText = "Sp_MST_QuestionLevel_Insert";
                    }
                    else // Update
                    {
                        command.CommandText = "Sp_MST_QuestionLevel_Update";
                        command.Parameters.Add("@QuestionLevelID", SqlDbType.Int).Value = model.QuestionLevelID;
                    }

                    // Common parameters
                    command.Parameters.Add("@QuestionLevel", SqlDbType.NVarChar).Value = model.QuestionLevelName;
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = model.UserID;

                    command.ExecuteNonQuery();
                }
                return RedirectToAction("QuestionLevelList");
            }

            return View("QuestionLevelForm", model);
        }
        public IActionResult ExportToExcel()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("Sp_MST_QuestionLevel_SelectAll", connection) // Updated stored procedure
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataReader reader = command.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("QuestionLevels");

                    // Add headers (without UserID)
                    worksheet.Cells[1, 1].Value = "QuestionLevelID";
                    worksheet.Cells[1, 2].Value = "QuestionLevel";
                    worksheet.Cells[1, 3].Value = "UserName"; // Kept UserName
                    worksheet.Cells[1, 4].Value = "Created";
                    worksheet.Cells[1, 5].Value = "Modified";

                    // Add data
                    int row = 2;
                    foreach (DataRow item in data.Rows)
                    {
                        worksheet.Cells[row, 1].Value = item["QuestionLevelID"];
                        worksheet.Cells[row, 2].Value = item["QuestionLevel"];
                        worksheet.Cells[row, 3].Value = item["UserName"]; // Fetching UserName
                        worksheet.Cells[row, 4].Value = Convert.ToDateTime(item["Created"]).ToString("yyyy-MM-dd HH:mm:ss"); // Formatting datetime
                        worksheet.Cells[row, 5].Value = Convert.ToDateTime(item["Modified"]).ToString("yyyy-MM-dd HH:mm:ss"); // Formatting datetime
                        row++;
                    }

                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0;

                    string excelName = $"QuestionLevels-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                }
            }
        }

        public IActionResult QuestionLevelSearch(int? questionLevelID, string username)
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("Sp_MST_QuestionLevel_Search", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@QuestionLevelID", (object)questionLevelID ?? DBNull.Value);
                command.Parameters.AddWithValue("@Username", (object)username ?? DBNull.Value);

                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);

                ViewBag.QuestionLevels = GetLevels(); // Load dropdown values
                ViewBag.Usernames = GetUsernames(); // Load username dropdown

                return View("QuestionLevelList", table);
            }
        }

        // Fetch Levels for Dropdown
        private List<Level> GetLevels()
        {
            List<Level> levels = new List<Level>();
            string connectionString = this.configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("Sp_MST_QuestionLevel_SelectfromDropdown", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    levels.Add(new Level
                    {
                        QuestionLevelID = Convert.ToInt32(reader["QuestionLevelID"]),
                        QuestionLevelName = reader["QuestionLevel"].ToString()
                    });
                }
            }
            return levels;
        }
        private List<AppUser> GetUsernames()  // Ensure it returns List<AppUser>
        {
            List<AppUser> users = new List<AppUser>();
            string connectionString = this.configuration.GetConnectionString("ConnectionString");

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand("Sp_MST_User_SelectforDropdown", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new AppUser  // Use AppUser instead of User
                            {
                                UserID = reader["UserID"] != DBNull.Value ? Convert.ToInt32(reader["UserID"]) : 0,
                                Username = reader["Username"]?.ToString() ?? "Unknown"
                            });
                        }
                    }
                }
            }
            return users;
        }

        public IActionResult SearchForm()
        {
            ViewBag.QuestionLevels = GetLevels();
            ViewBag.Usernames = GetUsernames();
            return View();
        }
    }
}



