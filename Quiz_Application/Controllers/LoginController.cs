using System.Data.SqlClient;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Quiz_Application.Models;

namespace Quiz_Application.Controllers
{
    public class LoginController : Controller
    {
        private IConfiguration Configuration;

        public LoginController(IConfiguration _configuration)
        {
            Configuration = _configuration;
        }

        public IActionResult Index()
        {
            return View("Login");
        }

        //[HttpPost]
        //public IActionResult Login(UserLoginModel userLoginModel)
        //{
        //    string ErrorMsg = string.Empty;

        //    if (string.IsNullOrEmpty(userLoginModel.UserName))
        //    {
        //        ErrorMsg += "User Name is Required";
        //    }

        //    if (string.IsNullOrEmpty(userLoginModel.Password))
        //    {
        //        ErrorMsg += "<br/>Password is Required";
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        SqlConnection conn = new SqlConnection(this.Configuration.GetConnectionString("ConnectionString"));
        //        conn.Open();

        //        SqlCommand objCmd = conn.CreateCommand();

        //        objCmd.CommandType = System.Data.CommandType.StoredProcedure;
        //        objCmd.CommandText = "Sp_MST_User_SelectByUserNamePassword";
        //        objCmd.Parameters.AddWithValue("@UserName", userLoginModel.UserName);
        //        objCmd.Parameters.AddWithValue("@Password", userLoginModel.Password);

        //        SqlDataReader objSDR = objCmd.ExecuteReader();

        //        DataTable dtLogin = new DataTable();

        //        // Check if Data Reader has Rows or not
        //        // if row(s) does not exists that means either username or password or both are invalid.
        //        if (!objSDR.HasRows)
        //        {
        //            TempData["ErrorMessage"] = "Invalid User Name or Password";
        //            return RedirectToAction("Login", "Login");
        //        }
        //        else
        //        {
        //            dtLogin.Load(objSDR);

        //            //Load the retrived data to session through HttpContext.
        //            foreach (DataRow dr in dtLogin.Rows)
        //            {
        //                HttpContext.Session.SetString("UserID", dtLogin.Rows[0]["UserID"].ToString());
        //                HttpContext.Session.SetString("UserName", dtLogin.Rows[0]["UserName"].ToString());
                       
        //            }
        //            return RedirectToAction("Index", "Home");
        //        }
        //    }
        //    else
        //    {
        //        TempData["ErrorMessage"] = ErrorMsg;
        //        return RedirectToAction("Login");
        //    }
        //}

        public IActionResult UserSave(UserLoginModel userLoginModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string connectionString = this.Configuration.GetConnectionString("ConnectionString");
                    SqlConnection sqlConnection = new SqlConnection(connectionString);
                    sqlConnection.Open();
                    SqlCommand sqlCommand = sqlConnection.CreateCommand();
                    sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
                    sqlCommand.CommandText = "Sp_MST_User_SelectByUserNamePassword";
                    sqlCommand.Parameters.Add("@UserName", SqlDbType.VarChar).Value = userLoginModel.UserName;
                    sqlCommand.Parameters.Add("@Password", SqlDbType.VarChar).Value = userLoginModel.Password;
                    SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();
                    DataTable dataTable = new DataTable();
                    dataTable.Load(sqlDataReader);
                    if (dataTable.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dataTable.Rows)
                        {
                            HttpContext.Session.SetString("UserID", dr["UserID"].ToString());
                            HttpContext.Session.SetString("UserName", dr["UserName"].ToString());
                        }

                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        return RedirectToAction("Index", "Login");
                    }

                }
            }
            catch (Exception e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return RedirectToAction("Index", "Login");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
