using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Quiz_Application.Models
{
    public class QuizWiseQuestion
    {
        public int? QuizWiseQuestionsID { get; set; }

        [Required(ErrorMessage = "Quiz ID is required")]
        [ForeignKey("Quiz")]
        public int QuizID { get; set; } // Foreign Key to MST_Quiz

        [Required(ErrorMessage = "Question ID is required")]
        [ForeignKey("Question")]
        public int QuestionID { get; set; } // Foreign Key to MST_Question

        [Required(ErrorMessage = "User ID is required")]
        [ForeignKey("User")]
        public int UserID { get; set; } // Foreign Key to MST_User

    }
}
