using Microsoft.AspNetCore.Mvc;

namespace Quiz_Application.Controllers
{
    public class MST_QuestionLevelController : Controller
    {
        public IActionResult QuestionLevelForm()
        {
            return View();
        }
        public IActionResult QuestionLevelList()
        {
            return View();
        }
    }
}
