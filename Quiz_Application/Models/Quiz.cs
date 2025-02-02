using System.ComponentModel.DataAnnotations;

namespace Quiz_Application.Models
{
    public class Quiz
    {
        [Required(ErrorMessage = "Quiz name is required")]
        [StringLength(100, ErrorMessage = "Quiz name cannot exceed 100 characters")]
        public string QuizName { get; set; }

        [Required(ErrorMessage = "Total questions are required")]
        [Range(1, int.MaxValue, ErrorMessage = "Total questions must be at least 1")]
        public int TotalQuestions { get; set; }

        [Required(ErrorMessage = "Quiz date is required")]
        [DataType(DataType.DateTime, ErrorMessage = "Invalid date format")]
        public DateTime QuizDate { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserID { get; set; } // Foreign Key to MST_User

    }
}
