using Microsoft.AspNetCore.Mvc;

namespace Quiz_Application.Controllers
{
    public class MST_QuestionController : Controller
    {
        public IActionResult QuestionForm()
        {
            return View();
        }
        public IActionResult QuestionList()
        {
            return View();
        }
    }
}
