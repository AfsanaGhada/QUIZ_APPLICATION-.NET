
using System.Data;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Quiz_Application.Models;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;


namespace Quiz_Application.Controllers
{
    public class MST_UserController : Controller
    {
        private IConfiguration configuration;

        public MST_UserController(IConfiguration _configuration)
        {
            configuration = _configuration;
        }
        public IActionResult UserForm(int UserID)
        {
            if (UserID == 0)
            {
                TempData["PageTitle"] = "Add User";
            }
            else
            {
                TempData["PageTitle"] = "Edit User";
            }

            ViewBag.UserID = UserID;
            string connectionString = configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand SqlCommand = connection.CreateCommand();
            SqlCommand.CommandType = CommandType.StoredProcedure;
            SqlCommand.CommandText = "Sp_MST_User_GetUser_ByID";
            SqlCommand.Parameters.AddWithValue("@UserID", UserID);
            SqlDataReader Reader = SqlCommand.ExecuteReader();
            DataTable Table = new DataTable();
            Table.Load(Reader);
            User Model = new User();

            foreach (DataRow dataRow in Table.Rows)
            {
                Model.UserID = Convert.ToInt32(@dataRow["UserID"]);
                Model.UserName = @dataRow["UserName"].ToString();
                Model.Email = @dataRow["Email"].ToString();

                Model.Password = @dataRow["Password"].ToString();
                Model.Mobile = @dataRow["Mobile"].ToString();

            }
            Console.WriteLine(Model);
            return View("UserForm", Model);
        }
        public IActionResult UserList()
        {
            string connectionString = this.configuration.GetConnectionString("ConnectionString");
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            SqlCommand command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "Sp_MST_User_SelectAllUsers";
            SqlDataReader reader = command.ExecuteReader();
            DataTable table = new DataTable();
            table.Load(reader);
            return View(table);
        }
        public IActionResult UserDelete(int UserID)
        {
            try
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = connection.CreateCommand();
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "Sp_MST_User_DeleteUserByID";
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;


                    command.ExecuteNonQuery();
                }

                TempData["SuccessMessage"] = "User deleted successfully.";
                return RedirectToAction("UserList");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while deleting the User: " + ex.Message;
                return RedirectToAction("UserList");
            }
        }


        public IActionResult UserAddEdit(int? UserID)
        {
            User model = new User();

            if (UserID != null && UserID > 0)
            {
                string connectionString = configuration.GetConnectionString("ConnectionString");
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();
                    SqlCommand command = new SqlCommand("Sp_MST_User_GetUser_ByID", connection);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = UserID;

                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        model.UserID = Convert.ToInt32(reader["UserID"]);
                        model.UserName = reader["UserName"].ToString();
                        model.Password = reader["Password"].ToString();
                        model.Email = reader["Email"].ToString();
                        model.Mobile = reader["Mobile"].ToString();

                        model.IsActive = Convert.ToBoolean(reader["IsActive"]);
                        model.IsAdmin = Convert.ToBoolean(reader["IsAdmin"]);

                    }
                }
            }
            return View("UserForm", model);
        }

        // POST: Save Question (Insert/Update)
        [HttpPost]
        public IActionResult UserSave(User model)
        {
            //if (ModelState.IsValid)
            //{
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.StoredProcedure
                };

                if (model.UserID == null || model.UserID == 0) // Insert
                {
                    command.CommandText = "Sp_MST_User_InsertNewUser";
                }
                else // Update
                {
                    command.CommandText = "Sp_MST_User_UpdateUser";
                    command.Parameters.Add("@UserID", SqlDbType.Int).Value = model.UserID;
                }

                // Common parameters
                command.Parameters.Add("@UserName", SqlDbType.NVarChar).Value = model.UserName;
                command.Parameters.Add("@Password", SqlDbType.NVarChar).Value = model.Password;
                command.Parameters.Add("@Email", SqlDbType.NVarChar).Value = model.Email;
                command.Parameters.Add("@Mobile", SqlDbType.NVarChar).Value = model.Mobile;

                command.Parameters.Add("@IsActive", SqlDbType.Bit).Value = model.IsActive;
                command.Parameters.Add("@IsAdmin", SqlDbType.Bit).Value = model.IsAdmin;


                command.ExecuteNonQuery();
                //}
                return RedirectToAction("UserList");
            }

            return View("UserForm", model);
        }

        

        public IActionResult ExportToExcel()
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("Sp_MST_User_SelectAllUsers", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                SqlDataReader reader = command.ExecuteReader();
                DataTable data = new DataTable();
                data.Load(reader);

                using (var package = new ExcelPackage())
                {
                    var worksheet = package.Workbook.Worksheets.Add("Users");

                    // Add headers
                    worksheet.Cells[1, 1].Value = "UserID";
                    worksheet.Cells[1, 2].Value = "UserName";
                    worksheet.Cells[1, 3].Value = "Email";
                    worksheet.Cells[1, 4].Value = "Mobile";
                    worksheet.Cells[1, 5].Value = "IsActive";
                    worksheet.Cells[1, 6].Value = "IsAdmin";

                    // Add data
                    int row = 2;
                    foreach (DataRow item in data.Rows)
                    {
                        worksheet.Cells[row, 1].Value = item["UserID"];
                        worksheet.Cells[row, 2].Value = item["UserName"];
                        worksheet.Cells[row, 3].Value = item["Email"];
                        worksheet.Cells[row, 4].Value = item["Mobile"];
                        worksheet.Cells[row, 5].Value = Convert.ToBoolean(item["IsActive"]) ? "Yes" : "No";
                        worksheet.Cells[row, 6].Value = Convert.ToBoolean(item["IsAdmin"]) ? "Yes" : "No";
                        row++;
                    }

                    var stream = new MemoryStream();
                    package.SaveAs(stream);
                    stream.Position = 0;

                    string excelName = $"Users-{DateTime.Now:yyyyMMddHHmmss}.xlsx";
                    return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", excelName);
                }
            }
        }
        public IActionResult UserSearch(int? userID, string userName, string mobile)
        {
            string connectionString = configuration.GetConnectionString("ConnectionString");
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlCommand command = new SqlCommand("Sp_MST_User_Search", connection)
                {
                    CommandType = CommandType.StoredProcedure
                };

                command.Parameters.AddWithValue("@UserID", (object)userID ?? DBNull.Value);
                command.Parameters.AddWithValue("@UserName", (object)userName ?? DBNull.Value);
                command.Parameters.AddWithValue("@Mobile", (object)mobile ?? DBNull.Value);

                SqlDataReader reader = command.ExecuteReader();
                DataTable table = new DataTable();
                table.Load(reader);

                return View("UserList", table); // Make sure you have a UserList.cshtml view
            }
        }
        public IActionResult UserRegister(UserRegisterModel userRegisterModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this.configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "[PR_MST_User_Register]";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userRegisterModel.UserName;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userRegisterModel.Password;
                    sqlCommand.Parameters.Add("@Email", SqlDbType.VarChar).Value = userRegisterModel.Email;
                    sqlCommand.Parameters.Add("@Mobile", SqlDbType.VarChar).Value = userRegisterModel.MobileNo;
                    //sqlCommand.Parameters.Add("@Address", SqlDbType.VarChar).Value = userRegisterModel.Address;
                    sqlCommand.ExecuteNonQuery();
                    return RedirectToAction("Login", "Index");
                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Register");
            }
            return RedirectToAction("Register");
        }
    }
   
}