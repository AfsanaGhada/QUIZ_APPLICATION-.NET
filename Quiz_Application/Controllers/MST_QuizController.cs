using Microsoft.AspNetCore.Mvc;

namespace Quiz_Application.Controllers
{
    public class MST_QuizController : Controller
    {
        public IActionResult QuizForm()
        {
            return View();
        }
        public IActionResult QuizList()
        {
            return View();
        }
    }
}
