using Microsoft.AspNetCore.Mvc;

namespace Quiz_Application.Controllers
{
    public class MST_QuizWiseQuestionsController : Controller
    {
        public IActionResult QuizWiseQuestionsForm()
        {
            return View();
        }
        public IActionResult QuizWiseQuestionsList()
        {
            return View();
        }
    }
    
}
