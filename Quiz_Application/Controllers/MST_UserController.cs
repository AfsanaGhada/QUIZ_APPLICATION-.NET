using Microsoft.AspNetCore.Mvc;

namespace Quiz_Application.Controllers
{
    public class MST_UserController : Controller
    {
        public IActionResult UserForm()
        {
            return View();
        }
        public IActionResult UserList()
        {
            return View();
        }
    }
}
